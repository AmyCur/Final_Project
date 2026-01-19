using Entities;
using UnityEngine;
using Elements;
using System.Collections;
using Combat.Enemies;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour {
	protected Rigidbody rb;

	protected enum Target{
		player,
		enemy,
		both
	}

	[SerializeField] protected float speed;
	[HideInInspector] public float damage;
	[HideInInspector] public Collider parent;
	[SerializeField] protected Target target=Target.both;

	bool canHitEntity = false;

	public virtual void FixedUpdate() {
		rb.linearVelocity = transform.forward * speed;
	}

	IEnumerator WaitAfterSpawn() {
		yield return new WaitForSeconds(0.7f);
		canHitEntity = true;
	}

	public virtual void OnTriggerEnter(Collider other) {
		if ((target == Target.player || target== Target.both) && other.isEntity<Player.PL_Controller>() && other != parent) {
			Entities.ENT_Controller ec = other.GetComponent<Entities.ENT_Controller>();
			ec ??= other.transform.parent.GetComponent<Entities.ENT_Controller>();
			ec.TakeDamage(damage, new Element(ElementType.None));
		}
		else if((target==Target.enemy || target==Target.both) && other.isEntity<ENM_Controller>()&& other != parent){
			Entities.ENT_Controller ec = other.GetComponent<Entities.ENT_Controller>();
			ec ??= other.transform.parent.GetComponent<Entities.ENT_Controller>();
			ec.TakeDamage(damage, new Element(ElementType.None));
		}


		if (
			other.isEntity<Player.PL_Controller>() ||
			(other.isEntity() && canHitEntity) ||
			(!other.isEntity() && other.tag != "Thought")
		) _="";
		//Destroy(gameObject);

	}

	IEnumerator DestroyBullet() {
		yield return new WaitForSeconds(30f);
		Destroy(gameObject);
	}

	void Awake() {
		if (TryGetComponent<Rigidbody>(out Rigidbody r)) rb = r;
		StartCoroutine(WaitAfterSpawn());
		StartCoroutine(DestroyBullet());
	}
}