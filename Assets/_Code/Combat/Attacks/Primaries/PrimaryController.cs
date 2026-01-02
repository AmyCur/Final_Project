using UnityEngine;
using MathsAndSome;
using EntityLib;
using System.Collections;
using Player;

[RequireComponent(typeof(Rigidbody))]
public class PrimaryController : MonoBehaviour
{
	GameObject player;
	Rigidbody rb;
	Vector3 direction;
	bool home;
	public float homeRange=3f;

	[SerializeField] float speed=1f;

	Vector3 Direction(){
		GameObject nearest = gameObject.GetNearestEnemy(10_000f,out _);
		if(nearest!=null && (nearest.transform.position-transform.position).magnitude <= homeRange) home=true;
		return (nearest==null || !home) ? direction : (-transform.position+nearest.transform.position).normalized;
		// return (player.transform.forward + Camera.main.transform.forward).normalized;
	}

	void Update() => rb.linearVelocity = Direction()*speed;

	void OnTriggerEnter(Collider other){
		if(other.isEntity(typeof(ENM_Controller))){
			ENM_Controller enm = other.GetComponent<ENM_Controller>();
			enm.health-=player.GetComponent<Combat.CombatController>().ca.primary.damage;
		}
		
		if(!other.isEntity(typeof(PL_Controller))) Destroy(gameObject);

	}


	void Start(){
		player = mas.player.Player.gameObject;
		rb=GetComponent<Rigidbody>();
		direction = Directions.forward; 
	}
}