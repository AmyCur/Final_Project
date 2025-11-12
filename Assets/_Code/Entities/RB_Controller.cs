using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RB_Controller : Entity.ENT_Controller {
	[Header("Movement")]
	public bool canMove = true;
	public Vector3 movementVector;

	[HideInInspector] public Rigidbody rb;

	[Header("Impulses")]
	public List<Impulse> SV;

	[System.Serializable]
	public class Impulse {
		public Vector3 force;
		readonly RB_Controller c;

		IEnumerator ReduceForce() {

			float dx = force.x / 2f;
			float dy = force.y / 2f;
			float dz = force.z / 2f;

			float ax = Mathf.Abs(force.x);
			float ay = Mathf.Abs(force.y);
			float az = Mathf.Abs(force.z);

			Vector3 sf = force;
			while (Mathf.Abs(force.x) + Mathf.Abs(force.y) + Mathf.Abs(force.z) > 1) {
				force = new(force.x - dx, force.y - dy, force.z - dz);

				force = new(
					Mathf.Clamp(force.x, sf.x < 0 ? -10_000 : 0, sf.x >= 0 ? 10_000 : 0),
					Mathf.Clamp(force.y, sf.y < 0 ? -10_000 : 0, sf.y >= 0 ? 10_000 : 0),
					Mathf.Clamp(force.z, sf.x < 0 ? -10_000 : 0, sf.z >= 0 ? 10_000 : 0)
				);

				yield return new WaitForSeconds(.1f);
			}
			c.SV.Remove(this);

		}

		public Impulse(Vector3 force, RB_Controller c) {
			this.c = c;
			this.force = force;
			mas.player.GetPlayer().StartCoroutine(ReduceForce());
		}
	}

	public void Move() {
		// I should have the player be moved by adding all the movement vectors
		Vector3 impulses() {
			Vector3 t = new();
			foreach (Impulse i in SV) {
				t += i.force;
			}
			return t;
		}

		Vector3 velocity = movementVector + impulses();

		if (velocity.y == 0) {
			velocity = new(velocity.x, rb.linearVelocity.y, velocity.z);
		}

		rb.linearVelocity = mas.vector.ClampVector(velocity, new Vector3[] { new(-1000, -40, -1000), new(1000, 20, 1000) });
	}

	public virtual void SetStartDefaults() {
		rb = GetComponent<Rigidbody>();
	}

	public override void Start() {
		base.Start();
		SetStartDefaults();
	}

	public virtual void FixedUpdate() {
		if (canMove) {
			Move();
		}
	}


}