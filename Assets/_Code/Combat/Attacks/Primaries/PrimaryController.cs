using UnityEngine;
using MathsAndSome;
using EntityLib;

[RequireComponent(typeof(Rigidbody))]
public class PrimaryController : MonoBehaviour
{
	GameObject player;
	Rigidbody rb;
	static Vector3 direction;

	[SerializeField] float speed=1f;

	Vector3 Direction(){
		GameObject nearest = gameObject.GetNearestEnemy(10_000f,out _);
		return nearest==null ? direction : (-transform.position+nearest.transform.position).normalized;
		// return (player.transform.forward + Camera.main.transform.forward).normalized;
	}

	void Update(){
		direction=Direction();
		rb.linearVelocity = direction*speed;
	}

	void OnCollisionEnter(Collision other){
		if(other.collider.isEntity(typeof(ENM_Controller))){
			ENM_Controller enm = other.collider.GetComponent<ENM_Controller>();
			enm.health-=player.GetComponent<Combat.CombatController>().ca.primary.damage;
		}
		Destroy(gameObject);

	}

	void Start(){
		player = mas.player.GetPlayer().gameObject;
		rb=GetComponent<Rigidbody>();
		direction = (player.transform.forward + Camera.main.transform.forward).normalized; 
	}
}