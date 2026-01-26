using UnityEngine;
using Entity;
using MathsAndSome;
using System.Collections;

public class GlassController : MonoBehaviour
{

	IEnumerator ResetPlayerVelocity(float v){
		Rigidbody rb = mas.player.Player.rb;
		yield return 0;
		rb.linearVelocity=new(rb.linearVelocity.x, v, rb.linearVelocity.z);
	}

	void OnCollisionEnter(Collision other){

		if (other.collider.isEntity<Player.Movement.PL_Controller>()){
			float velocity = mas.player.Player.rb.linearVelocity.y;
			if(mas.player.Player.state==Player.PlayerState.slamming) {
				Destroy(gameObject);
				StartCoroutine(ResetPlayerVelocity(velocity));
			}
		}
	}
}
