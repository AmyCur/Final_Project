using UnityEngine;
using MathsAndSome;

namespace Player.Movement{
	public class PL_Utility : MonoBehaviour{
		void OnCollisionStay(Collision other){
			Dash.dashForceToAdd = mas.vector.LerpVectors(Dash.dashForceToAdd, Vector3.zero, Time.deltaTime*2f); 
		}		
	}	
}