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
        if (other.isEntity(typeof(Player.PL_Controller)) && other != parent) {
            Debug.Log(other.name);
            ENT_Controller ec = other.GetComponent<ENT_Controller>();
            ec ??= other.transform.parent.GetComponent<ENT_Controller>();
            ec.TakeDamage(damage, new Element(ElementType.None));
        }
        if (other.isEntity(typeof(Player.PL_Controller))) Destroy(gameObject);
    }

    void Awake() {
        if (TryGetComponent<Rigidbody>(out Rigidbody r)) rb = r;
    }
}