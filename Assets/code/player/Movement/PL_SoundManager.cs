using Audio;
using MathsAndSome;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Player.Movement{
	public static class PL_SoundManager{
		public static PL_Controller pc => mas.player.Player;

		public static float WalkPercentage=0f;
		public static System.Random rand;
		public const float footstepTriggerValue=3f;


		public static void PlayFootStepNoise(){
			AudioManager.PlaySoundUntilStop(pc.footsteps[rand.Next(0, pc.footsteps.Count)]);

		}

		public static IEnumerator FootstepRoutine(){
			while(true){
				while(pc.rb.linearVelocity.magnitude > 1f && pc.Grounded()){
					WalkPercentage+=0.01f;
					if(WalkPercentage==footstepTriggerValue){
						PlayFootStepNoise();
					}
					yield return 0;
				}
				yield return 0;
			}
			
		}
		
		[RuntimeInitializeOnLoadMethod]
		public static void Start(){
			rand=new System.Random();
			pc.StartCoroutine(FootstepRoutine());
		}
	}
}