using Globals;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireballController : ProjectileController {

    [SerializeField] float speed;
    public Vector3 direction;
    Rigidbody rb;

    protected override void Start() {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Update() {
        base.Update();
        rb.linearVelocity = direction * speed;
    }

    protected override void DestroyObject() {
           
    }


	void OnCollisionEnter(Collision other) {
        if (other.collider.tag != glob.playerTag) {
            DestroyObject();
        }
	}
}