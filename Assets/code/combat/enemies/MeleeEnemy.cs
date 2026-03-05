using UnityEngine;
using System.Collections;
using Entity;
using MathsAndSome;

namespace Combat.Enemies{
	public sealed class MeleeEnemy : ENM_Controller
	{
		protected override void Hunt(){
			agent.destination = pc.transform.position - ((transform.position-pc.transform.position).normalized*3f);
		}

		protected override void Attack(){
			if (canAttack){
				gameObject.ShootPlayer<RaycastHit>(range, damage);
				base.Attack();
			}
		}

		protected override bool ShouldAttack(){
			bool playerInRange=false;

			if(Physics.Raycast(transform.position, playerPosition-transform.position, out RaycastHit hit, range)){
				playerInRange = hit.isEntity<Player.Movement.PL_Controller>();
			}
			
			return playerInRange;
		}

		protected override bool ShouldHunt(){
			return !ShouldAttack();
		}

		void OnDrawGizmos(){
			if (mas.player.Player != null){
				Debug.DrawRay(transform.position, (mas.player.Player.transform.position-transform.position)*range);			
			}
		}
	}    
}