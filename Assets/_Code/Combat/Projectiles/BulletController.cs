using EntityLib;
using UnityEngine;
using Elements;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour {
    Rigidbody rb;

    [SerializeField] float speed;
    [HideInInspector] public float damage;
    [HideInInspector] public Collider parent;

    void FixedUpdate() {
        rb.linearVelocity = transform.forward * speed;
    }

	void OnTriggerEnter(Collider other) {
        if (other.isEntity() && other!=parent) other.GetComponent<ENT_Controller>().TakeDamage(damage, new Element(ElementType.None));
        
        Destroy(gameObject);
	}

	void Awake() {
        if (TryGetComponent<Rigidbody>(out Rigidbody r)) rb = r;
	}
}