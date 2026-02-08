using MathsAndSome;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Player.Movement;

public static class Dash{
	static PL_Controller pc => mas.player.Player;
	static bool canBreakFromGravityDash = false;
	static bool noGravTimerOver = false;

	
	public static void HandleDash() {
		pc.dash.state = MovementState.start;
		pc.stamina.Subtract(pc.dash.staminaPer);
		pc.StartCoroutine(NoGravityDash());
	}


	public static Vector3 dashForceToAdd = Vector3.zero;

	static bool shouldStopDash=false;

	public static async void StopDash(){
		dashForceToAdd=Vector3.zero;
		shouldStopDash=true;
		await Task.Delay(200);
		shouldStopDash=false;
	}

	public static float force;


	public static IEnumerator DecayDash(float decaySpeed = -1f) {
		if (decaySpeed == -1f) decaySpeed = pc.dash.decaySpeed;
		force = pc.dash.force;

		// idk if this needs to be a do while
		do {
			dashForceToAdd = pc.dash.direction * force * 1_000f * Time.deltaTime;
			// Debug.Log(forceToAdd);
			pc.rb.AddForce(dashForceToAdd);
			force = Mathf.Lerp(force, 0, Time.deltaTime * decaySpeed);
			yield return 0;
		} while (force > 0.1f && !pc.shouldDash && pc.state != PlayerState.slamming && pc.adminState != AdminState.noclip && !shouldStopDash);

		pc.dash.state = MovementState.none;

	}


	static IEnumerator WaitForGravityDash() {
		yield return new WaitForSeconds(0.1f);
		canBreakFromGravityDash = true;
	}


	public static IEnumerator GravityDash() {
		pc.rb.linearVelocity=new Vector3(0,pc.rb.linearVelocity.y,0);
		canBreakFromGravityDash = false;

		pc.StartCoroutine(WaitForGravityDash());

		float force=pc.dash.gravityDashForce;

		do {
			if(shouldStopDash) break;
			pc.rb.AddForce(pc.dash.direction * force * Consts.Multipliers.DASH_MULTIPLIER * Time.deltaTime, ForceMode.Acceleration);


			yield return 0;

			// if ((Grounded() && canBreakFromSlidePreservation) || shouldDash) break;

		} while (!((pc.Grounded() && canBreakFromGravityDash) || pc.shouldDash || pc.state == PlayerState.slamming || pc.adminState == AdminState.noclip));

		pc.dash.state = MovementState.end;

		if(!shouldStopDash){
			if (pc.Grounded()) pc.StartCoroutine(DecayDash(decaySpeed: pc.slide.decaySpeed));
		}

		pc.dash.state = MovementState.none;

	}

	static IEnumerator SetNoGravTimerOver(){
		noGravTimerOver=false;
		yield return new WaitForSeconds(pc.dash.noGravDashTime);
		noGravTimerOver=true;
	}

	static IEnumerator DashCD(){
		pc.dash.can = false;
		yield return new WaitForSeconds(0.2f);
		pc.dash.can = true;
	}

	// Dash where gravity is disabled (The only dash if youre on the ground)
	public static IEnumerator NoGravityDash() {
		pc.rb.useGravity = false;
		yield return 0;
		pc.rb.linearVelocity=Vector3.zero;

		pc.StartCoroutine(SetNoGravTimerOver());

		pc.dash.direction = Directions.DashDirection(pc, true);

		pc.rb.linearVelocity = Vector3.zero;
		
		force = pc.dash.force;

		while (!noGravTimerOver && pc.state != PlayerState.slamming && pc.adminState != AdminState.noclip && !shouldStopDash) {

			pc.rb.AddForce(pc.dash.direction * force * Time.deltaTime * Consts.Multipliers.DASH_MULTIPLIER, ForceMode.Acceleration);

			yield return 0;
		}

		pc.dash.state = MovementState.middle;


		pc.rb.useGravity = true;

		if (!pc.Grounded()) pc.StartCoroutine(GravityDash());
		else pc.StartCoroutine(DecayDash());
	}
}
