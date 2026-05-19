using UnityEngine;
using System.Collections;
using Entity;
using MathsAndSome;
using Audio;
using Player.Movement;

namespace Combat.Enemies{
	public sealed class MeleeEnemy : ENM_Controller
	{


		protected override void Hunt(){
			// AudioManager.PlaySoundUntilStop(huntSound);
			agent.destination = pc.transform.position - ((transform.position-pc.transform.position).normalized*3f);
		}

		protected override void Attack(){
			if (canAttack){
			
					Debug.Log("ATK");
				
				RaycastHit[] hits = Physics.RaycastAll(transform.position, pc.transform.position-transform.position, range);
				foreach(RaycastHit hit in hits){
					Debug.Log(hit.transform.name);
					if(!hit.collider.isTrigger){
						if(hit.isEntity<ENM_Controller>()){

						}
						else if(hit.isEntity<PL_Controller>()){
							pc.health-=damage;
						}
						else{
							break;
						}
					}
				}
				
				// gameObject.ShootPlayer<RaycastHit>(range, damage);
				base.Attack();
			}
		}

		protected override bool ShouldAttack(){
			bool playerInRange=false;

			// if(Physics.Raycast(transform.position, playerPosition-transform.position, out RaycastHit hit, range)){
			// 	playerInRange = hit.isEntity<Player.Movement.PL_Controller>();
			// }
			// HACK HACK HACK
			// Debug.Log((pc.transform.position-transform.position).magnitude);
			return (pc.transform.position-transform.position).magnitude <= range;
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