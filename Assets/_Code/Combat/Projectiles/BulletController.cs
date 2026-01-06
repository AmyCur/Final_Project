using EntityLib;
using UnityEngine;
using Elements;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour {
	Rigidbody rb;

	[SerializeField] float speed;
	[HideInInspector] public float damage;
	[HideInInspector] public Collider parent;

	bool canHitEntity = false;
	void FixedUpdate() {
		rb.linearVelocity = transform.forward * speed;
	}

	IEnumerator WaitAfterSpawn() {
		yield return new WaitForSeconds(0.7f);
		canHitEntity = true;
	}

	void OnTriggerEnter(Collider other) {
		if (other.isEntity<Player.PL_Controller>() && other != parent) {
			Entity.ENT_Controller ec = other.GetComponent<Entity.ENT_Controller>();
			ec ??= other.transform.parent.GetComponent<Entity.ENT_Controller>();
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