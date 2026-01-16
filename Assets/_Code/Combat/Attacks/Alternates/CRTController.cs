using UnityEngine;
using System.Collections;
using Combat.Enemies;


namespace Combat.Attacks{
	public class CRTController : MonoBehaviour{

		[SerializeField] float maxLifeTime=30f;
		[SerializeField] LayerMask groundMask;
		[HideInInspector] public float damage;

		IEnumerator WaitForGrounded(){
			print("wait");
			yield return new WaitUntil(() => Physics.Raycast(transform.position, Vector3.down, 1f, groundMask));
			Destroy(gameObject);
		}

		IEnumerator EmergencyStuckCase(){
			yield return new WaitForSeconds(maxLifeTime);	
			Destroy(gameObject);
		}
		
		void Start(){
			StartCoroutine(EmergencyStuckCase());
			StartCoroutine(WaitForGrounded());
		}

		void OnTriggerEnter(Collider other){
			if(other.TryGetComponent<ENM_Controller>(out ENM_Controller ec)){
				ec.health-=damage;
			}
		}
	}
}