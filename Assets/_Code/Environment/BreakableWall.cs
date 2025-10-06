using System;
using UnityEngine;



public class BreakableWall : ENT_Controller {
    public override void Start() {
        base.Start();
        if (!!GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());
    }


    public override void Die() {
        base.Die();
        Debug.Log("die");
        Destroy(gameObject);
    }
}
