using UnityEngine;
using System.Collections;
using MathsAndSome;
using Player;
using EntityLib;

public class BoomerangController : BulletController
{
	Vector3 forward;

	[SerializeField] float initialTurnAroundTime=1f;
	[SerializeField] float lerpSpeed = 10f;
	bool destroyIfTouchingPlayer;
	float moveSpeed=1f;

	IEnumerator TurnAround(){
		yield return new WaitForSeconds(initialTurnAroundTime);
		Vector3 target = -forward;
		Vector3 f = forward;
		destroyIfTouchingPlayer=true;
		while (forward != target){
		transform.rotation =  Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, -mas.player.Player.playerCamera.transform.forward, 10f*Time.deltaTime, 0f));
			print(moveSpeed);
			forward = mas.vector.LerpVectors(forward, (-transform.position+mas.player.Player.transform.position).normalized*10f, Time.deltaTime*lerpSpeed*moveSpeed);
			moveSpeed+=0.01f;
			yield return 0;
		}
	}

	public override void FixedUpdate(){
		rb.linearVelocity=forward*moveSpeed;
	}

	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		if(destroyIfTouchingPlayer && other.isEntity<PL_Controller>()){
			Destroy(gameObject);
		}  
	}

    void Start(){
		forward = transform.forward * speed;
		StartCoroutine(TurnAround());
	}
}
