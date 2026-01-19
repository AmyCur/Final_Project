using Combat.Enemies;
using EntityLib;
using MathsAndSome;
using UnityEngine;

namespace Combat.Attacks.Projectiles;

public class BulletController : MonoBehaviour {
	protected Rigidbody rb;
	protected Vector3 direction;
	[HideInInspector] public float damage=5f;
	[SerializeField] protected float moveSpeed=5f;
	[SerializeField] List<EnemyType> target;

	public virtual void Init(Vector3 direction, float damage){
		this.direction=direction;
		this.damage=damage;
	}
	
	protected virtual void FixedUpdate(){
		rb.linearVelocity=direction*moveSpeed;
	}


	protected virtual void HitPlayer(){
		if (other.isEntity<Player.PL_Controller>()){
			mas.player.Player.health-=damage;
		}
	}

	protected virtual void HitEnemy(){
		if(other.isEntity<ENM_Controller>()){
			ENM_Controller ec = other.GetComponent<ENM_Controller>();
			ec.health-=damage;
		}
	}

	protected virtual void OnTriggerEnter(Collider other){
		foreach(EnemyType et in target){
			switch (et){
				case EnemyType.player:
					HitPlayer();
					break;
				case EnemyType.enemy:
					HitEnemy();
					break;		
			}
		}
	}

	protected virtual void Start(){
		rb=GetComponent<Rigidbody>();
	}
}
