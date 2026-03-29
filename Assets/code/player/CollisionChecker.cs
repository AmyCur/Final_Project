using MathsAndSome;
using UnityEngine;

namespace Player{
	public class CollisionChecker : MonoBehaviour {
		Player.Movement.PL_Controller pc;

		void OnTriggerStay(Collider other) {
			if (!pc.shouldDash) {
				pc.dash.force = 0f;
			}

			Vector3[] clamp = {new Vector3(-1000,-1000,-1000), new Vector3(pc.rb.linearVelocity.x,Mathf.Clamp(pc.rb.linearVelocity.y-1f,0,1000),pc.rb.linearVelocity.z)};
			pc.rb.linearVelocity=mas.vector.ClampVector(pc.rb.linearVelocity, clamp);
		}

		void Start() {
			pc = mas.player.Player;
		}
	}
}
