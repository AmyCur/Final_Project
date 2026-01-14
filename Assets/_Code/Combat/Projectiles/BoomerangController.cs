using UnityEngine;
using System.Collections;
using MathsAndSome;

public class BoomerangController : BulletController
{
	Vector3 forward;

	[SerializeField] float initialTurnAroundTime=1f;
	[SerializeField] float lerpSpeed = 10f;

	IEnumerator TurnAround(){
		yield return new WaitForSeconds(initialTurnAroundTime);
		Vector3 target = -forward;
		Vector3 f = forward;
		while (forward != target){
			forward = mas.vector.LerpVectors(forward, (-transform.position+mas.player.Player.transform.position).normalized*10f, Time.deltaTime*lerpSpeed);
			yield return 0;
		}
	}

	public override void FixedUpdate(){
		rb.linearVelocity=forward;
	}

    void Start(){
		forward = transform.forward * speed;
		StartCoroutine(TurnAround());
	}
}
