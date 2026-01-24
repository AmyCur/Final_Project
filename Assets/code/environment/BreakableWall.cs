using System;
using UnityEngine;
using Entity;



public class BreakableWall : Entity.ENT_Controller {
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