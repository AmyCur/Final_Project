using Combat.Enemies;
using Entity;
using UnityEngine;
using System.Collections.Generic;
using Entities;
using MathsAndSome;
using Combat.Elements;

namespace Combat.Attacks.Projectiles{
	public class BulletController : MonoBehaviour{
		protected Rigidbody rb;
		protected Vector3 direction;
		public Element element;
		[HideInInspector] public float damage=5f;
		[SerializeField] protected float moveSpeed=5f;
		[SerializeField] List<EnemyTypes> targetTypes;

		public virtual void Init(Vector3 dir, float dmg){
			direction=dir;
			damage=dmg;
		}
		
		protected virtual void FixedUpdate(){
			rb.linearVelocity=direction* moveSpeed;
		}

		protected virtual void OnTriggerEnter(Collider other){
			foreach(EnemyTypes eType in targetTypes){
				if (eType == EnemyTypes.player){
					if(other.isEntity<Player.Movement.PL_Controller>()){
						mas.player.Player.health-=damage;
					}
				}
				else if(eType == EnemyTypes.enemy){
					if(other.isEntity<ENM_Controller>()){
						ENM_Controller ec = other.GetComponent<ENM_Controller>();
						ec.TakeDamage(damage, element);
					}
				}
			}		
		}

		protected virtual void Start(){
			rb=GetComponent<Rigidbody>();
		}
	}
}