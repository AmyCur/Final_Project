using MathsAndSome;
using UnityEngine;

namespace Player.Movement;

public static class Slam{
	static PL_Controller pc => mas.player.Player;


	public static void HandleSlam() {
		pc.state = PlayerState.slamming;
		pc.rb.linearVelocity = Vector3.zero;
		pc.rb.AddForce(new Vector3(0, -pc.slam.force * 5f * Time.deltaTime, 0), ForceMode.Impulse);
		
	}	
}
