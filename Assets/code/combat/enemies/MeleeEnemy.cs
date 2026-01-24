using UnityEngine;
using System.Collections;
using EntityLib;

namespace Combat.Enemies{
	public sealed class MeleeEnemy : ENM_Controller
	{
		// Called every frame
		protected override void Hunt(){
			agent.destination = pc.transform.position - ((transform.position-pc.transform.position).normalized*3f);
		}

		// Called every frame
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
	}    
}