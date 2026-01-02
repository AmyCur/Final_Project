using MathsAndSome;
using System.Collections;
using UnityEngine;

namespace Player{
	public static class Dash{
		static PL_Controller pc => mas.player.Player;
		static bool canBreakFromGravityDash = false;
		static bool noGravTimerOver = false;

		
		public static void HandleDash() {
			pc.dash.state = MovementState.start;
			pc.stamina.Subtract(pc.dash.staminaPer);
			pc.StartCoroutine(NoGravityDash());
		}

		public static IEnumerator DecayDash(float decaySpeed = -1f) {
			if (decaySpeed == -1f) decaySpeed = pc.dash.decaySpeed;
			float force = pc.dash.force;

			// idk if this needs to be a do while
			do {
				Vector3 forceToAdd = pc.dash.direction * force * 1_000f * Time.deltaTime;
				// Debug.Log(forceToAdd);
				pc.rb.AddForce(forceToAdd);
				force = Mathf.Lerp(force, 0, Time.deltaTime * decaySpeed);
				yield return 0;
			} while (force > 0.1f && !pc.shouldDash && pc.state != PlayerState.slamming && pc.adminState != AdminState.noclip);

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


			do {
				pc.rb.AddForce(pc.dash.direction * pc.dash.gravityDashForce * Consts.Multipliers.DASH_MULTIPLIER * Time.deltaTime, ForceMode.Acceleration);


				yield return 0;

				// if ((Grounded() && canBreakFromSlidePreservation) || shouldDash) break;

			} while (!((pc.Grounded() && canBreakFromGravityDash) || pc.shouldDash || pc.state == PlayerState.slamming || pc.adminState == AdminState.noclip));

			pc.dash.state = MovementState.end;


			if (pc.Grounded()) pc.StartCoroutine(DecayDash(decaySpeed: pc.slide.decaySpeed));

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
			while (!noGravTimerOver && pc.state != PlayerState.slamming && pc.adminState != AdminState.noclip) {

				pc.rb.AddForce(pc.dash.direction * pc.dash.force * Time.deltaTime * Consts.Multipliers.DASH_MULTIPLIER, ForceMode.Acceleration);

				yield return 0;
			}

			pc.dash.state = MovementState.middle;


			pc.rb.useGravity = true;

			if (!pc.Grounded()) pc.StartCoroutine(GravityDash());
			else pc.StartCoroutine(DecayDash());
		}
	}
}