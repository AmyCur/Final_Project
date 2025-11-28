using UnityEngine;
using MathsAndSome;
using EntityLib;

[RequireComponent(typeof(Rigidbody))]
public class PrimaryController : MonoBehaviour
{
	GameObject player;
	Rigidbody rb;
	Vector3 direction;

	[SerializeField] float speed=1f;

	Vector3 Direction(){
		GameObject nearest = gameObject.GetNearestEnemy(10_000f,out _);
		return nearest==null ? Vector3.zero : (-transform.position+nearest.transform.position).normalized;
		// return (player.transform.forward + Camera.main.transform.forward).normalized;
	}

	void Update(){
		direction=Direction();
		rb.linearVelocity = direction*speed;
	}

	void Start(){
		player = mas.player.GetPlayer().gameObject;
		rb=GetComponent<Rigidbody>();
	}
}