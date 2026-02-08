using UnityEngine;
using MathsAndSome;

namespace Player.Movement{
	public class PL_Utility : MonoBehaviour{
		[SerializeField] float DashForceDecaySpeed=10f;

		void OnTriggerStay(Collider other){
			Dash.force = Mathf.Lerp(Dash.force, 0, Time.deltaTime*DashForceDecaySpeed); 
		}	

		// void OnTriggerEnter(Collider other){
		// 	Dash.StopDash();
		// }		
	}	
}