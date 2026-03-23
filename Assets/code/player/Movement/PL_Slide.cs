using Magical;
using MathsAndSome;
using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Player.Movement;

public static class Slide{

	static PL_Controller pc => mas.player.Player;
	static bool canBreakFromSlidePreservation = false;
	static bool shouldStopSlide=false;
	
	public static async void StopSlide(){
		shouldStopSlide = true;
		await Task.Delay(200);
		shouldStopSlide =false;
	}

	public static IEnumerator DecaySlide(float decaySpeed = -1f) {
		if (decaySpeed == -1f) decaySpeed = pc.slide.decaySpeed;
		float force = pc.slide.force;

		// idk if this needs to be a do while
		do {
			pc.rb.AddForce(pc.slide.direction * force * Consts.Multipliers.SLIDE_MULTIPLIER * Time.deltaTime);
			force = Mathf.Lerp(force, 0, Time.deltaTime * decaySpeed);
			// Debug.LogWarning(force);
			// ???? Couldve just used yield return 0
			yield return new WaitForSeconds(decaySpeed / (float) pc.slide.decayIncrements);
		} while (force > 0.1f && pc.state != PlayerState.slamming);
	}

	static IEnumerator WaitForSlideJumpPreservation() {
		yield return new WaitForSeconds(0.1f);
		canBreakFromSlidePreservation = true;
	}

	public static IEnumerator PreserveSlideJump(Vector3 direction) {
		canBreakFromSlidePreservation = false;

		pc.StartCoroutine(WaitForSlideJumpPreservation());

		do {
			pc.rb.AddForce(direction * pc.slide.force * Consts.Multipliers.SLIDE_MULTIPLIER * Time.deltaTime * .8f);
			direction = pc.slide.direction;
			yield return 0;

			// if ((Grounded() && canBreakFromSlidePreservation) || shouldDash) break;
		} while (!((pc.Grounded() && canBreakFromSlidePreservation) || pc.shouldDash || pc.state == PlayerState.slamming || pc.adminState == AdminState.noclip));

		if (pc.Grounded()) pc.StartCoroutine(DecaySlide(decaySpeed: pc.slide.decaySpeed));
	}
	
	
	static Coroutine cameraSlideRoutine;

	public static IEnumerator LerpCameraSlide(bool down){
		
		while(pc.playerCamera.transform.position != (down ? pc.slideCameraObject.transform.position : pc.defaultCameraObject.transform.position)){
			Vector3 pos = pc.playerCamera.transform.position;
			pc.playerCamera.transform.position = new Vector3(
				pos.x,
				Mathf.Lerp(pos.y, down ? pc.slideCameraObject.transform.position.y : pc.defaultCameraObject.transform.position.y, Time.deltaTime*20f),
				pos.z
			);
			yield return 0;
		}
	}

	static async void WaitForJustSlid()
	{
		GameObject.Find("Legs").GetComponent<Animator>().SetBool("just_slid", true);
		await Task.Delay(100);
		GameObject.Find("Legs").GetComponent<Animator>().SetBool("just_slid", false);
	}

	static bool CheckForSlideNotMoving(){
		// Debug.Log($"{pc.rb.linearVelocity.magnitude} : {pc.slide.force}");
		return pc.rb.linearVelocity.magnitude < pc.slide.force;
	}

	// static async Task<bool> WaitForSlideChange(){
	// 	for(int i = 0; i < 10; i++){
	// 		if(CheckForSlideNotMoving()){
	// 			if(i==10){
	// 				return true;
	// 			}
	// 		}
	// 		else{
	// 			break;
	// 		}

	// 		await Task.Delay(50);
	// 	}

	// 	return false;
	// }

	static IEnumerator WaitForSlideChange(){
		Debug.Log("AAA");
		for(int i = 0; i < 10; i++){
			if(CheckForSlideNotMoving()){
				if(i==9){
					shouldStopSliding = true;
					Debug.Log("AAAA");
				}
			}
			else{
				break;
			}

			yield return new WaitForSeconds(0.03f);
		}

		shouldStopSliding = false;
	}


	static bool shouldStopSliding=false;

	public static IEnumerator SlideRoutine() {

		pc.justSlid=false;
		
		GameObject.Find("Legs").GetComponent<Animator>().SetBool("legs_out", true);

		pc.collider.height=1;

		pc.slide.direction = Directions.SlideDirection(pc);
		Vector3 direction = pc.slide.direction;

		pc.state = PlayerState.sliding;

		if(cameraSlideRoutine!=null) pc.StopCoroutine(cameraSlideRoutine);
		cameraSlideRoutine = pc.StartCoroutine(LerpCameraSlide(true));

		do {
			if(CheckForSlideNotMoving()){
				pc.StartCoroutine(WaitForSlideChange());
			}
			pc.rb.AddForce(direction * (pc.slide.force/*+finalSlamVelocity*/) * Consts.Multipliers.SLIDE_MULTIPLIER * Time.deltaTime);
			yield return 0;
			GameObject.Find("Legs").GetComponent<Animator>().SetBool("sliding", true);
			
			
		} while (magic.key.gk(keys.slide) && !pc.shouldJump  && pc.state != PlayerState.slamming && pc.adminState != AdminState.noclip && !shouldStopSliding && pc.dash.state != MovementState.start);

		GameObject.Find("Legs").GetComponent<Animator>().SetBool("sliding", false);
		GameObject.Find("Legs").GetComponent<Animator>().SetBool("legs_out", false);
		WaitForJustSlid();


		if(cameraSlideRoutine!=null) pc.StopCoroutine(cameraSlideRoutine);
		cameraSlideRoutine = pc.StartCoroutine(LerpCameraSlide(false));

		// Handle Slide End based on how the slide has ended

		// No Grounded -> Medium decay
		// if (!Grounded()) StartCoroutine(DecaySlide(direction: slideDirection, decaySpeed: 2f)); 

		// Jumped -> No decay
		if (pc.shouldJump) { pc.StartCoroutine(PreserveSlideJump(direction)); }
		else if (!pc.Grounded(1.3f)) pc.StartCoroutine(PreserveSlideJump(direction));

		// No longer pressing slide -> Fast decay
		else if (!magic.key.gk(keys.slide)) pc.StartCoroutine(DecaySlide(decaySpeed: pc.slide.decaySpeed));

		pc.state = PlayerState.walking;
		pc.justSlid=true;
		pc.collider.height=2;
		yield return new WaitForSeconds(0.2f);

		pc.justSlid=false;
	}

}
