using UnityEngine;
using MathsAndSome;

namespace Player.Movement{
	public class PL_Utility : Singleton<PL_Utility>{
		[SerializeField] float DashForceDecaySpeed=10f;

		// void OnTriggerStay(Collider other){
		// 	Dash.force = 0;
		// }	

		void OnTriggerEnter(Collider other){
			Dash.StopDash();
			Slide.StopSlide();
		}		
	}	
}