using Combat.Enemies;
using EntityLib;
using UnityEngine;

namespace Combat.Attacks.Projectiles{
	public class BulletController : MonoBehaviour{
		protected Rigidbody rb;
		protected Vector3 direction;
		[HideInInspector] public float damage=5f;
		[SerializeField] protected float moveSpeed=5f;

		public virtual void Init(Vector3 direction, float damage){
			this.direction=direction;
			this.damage=damage;
		}
		
		protected virtual void FixedUpdate(){
			rb.linearVelocity=direction*moveSpeed;
		}

		protected virtual void OnTriggerEnter(Collider other){
			if(other.isEntity<ENM_Controller>()){
				ENM_Controller ec = other.GetComponent<ENM_Controller>();
				ec.health-=damage;
			}
		}

		protected virtual void Start(){
			rb=GetComponent<Rigidbody>();
		}
	}
}