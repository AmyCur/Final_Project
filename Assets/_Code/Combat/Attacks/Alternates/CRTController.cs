using UnityEngine;
using System.Collections;


namespace Combat.Attacks{
	public class CRTController : MonoBehaviour{

		[SerializeField] float maxLifeTime=30f;
		[SerializeField] LayerMask groundMask;

		IEnumerator WaitForGrounded(){
			print("wait");
			yield return new WaitUntil(() => Physics.Raycast(transform.position, Vector3.down, 1f, groundMask));
			Destroy(gameObject);
		}

		IEnumerator EmergencyStuckCase(){
			yield return new WaitForSeconds(maxLifeTime);	
		}
		
		void Start(){
			StartCoroutine(EmergencyStuckCase());
			StartCoroutine(WaitForGrounded());
		}
	}
}