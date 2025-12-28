using System;
using System.Collections;
using UnityEngine;


namespace Entity {
	[Serializable]
	public class Health {

		public float h;
		public float maxHealth = 100f;
		public bool canTakeDamage = true;
		[HideInInspector] public bool takenDamage;

		public static Health operator +(Health h, object h2) {
			Health h3 = new();

			if (h2 is Health health) h3.h = h.h + health.h;
			else if (h2 is float hF) h3.h = h.h + hF;

			if (h3.h > h3.maxHealth) h3.h = h3.maxHealth;

			return h3;
		}

		public static Health operator /(Health h, object h2) {
			Health h3 = new();
			if (h2 is Health health) h3.h = h.h / health.h;
			else if (h2 is float hF) h3.h = h.h / hF;
			return h3;
		}

		public static Health operator *(Health h, object h2) {
			Health h3 = new();
			if (h2 is Health health) h3.h = h.h * health.h;
			else if (h2 is float hF) h3.h = h.h * hF;
			return h3;
		}

		public static bool operator <=(Health h1, object h2) {
			if (h2 is Health health) return h1.h <= health.h;
			else if (h2 is float hF) return h1.h <= hF;
			else if (h2 is int hI) return h1.h <= (float) hI;
			return false;
		}

		public static bool operator <(Health h1, object h2) {
			if (h2 is Health health) return h1.h < health.h;
			else if (h2 is float hF) return h1.h < hF;
			return false;
		}

		public static bool operator >(Health h1, object h2) {
			if (h2 is Health health) return h1.h > health.h;
			else if (h2 is float hF) return h1.h > hF;
			return false;
		}

		public static bool operator >=(Health h1, object h2) {
			if (h2 is Health health) return h1.h >= health.h;
			else if (h2 is float hF) return h1.h >= hF;
			else if (h2 is int hI) return h1.h >= (float) hI;

			return false;
		}

		public static bool operator ==(Health h1, object h2) {
			if (h2 is Health health) return h1.h == health.h;
			else if (h2 is float hF) return h1.h == hF;
			return false;
		}

		public static bool operator !=(Health h1, object h2) {
			if (h2 is Health health) return h1.h != health.h;
			else if (h2 is float hF) return h1.h != hF;
			return false;
		}

		public static Health operator -(Health h, object h2) {
			Health h3 = h;

			if (h.canTakeDamage) {
				if (h2 is Health health) h3.h -= health.h;
				else if (h2 is float hF) h3.h -= hF;

				h3.takenDamage = true;
			}

			return h3;
		}

		public override bool Equals(object obj) => obj is Health health && this.h == health.h;
		public override int GetHashCode() => HashCode.Combine(h);
	}
}

namespace Player {
	[Serializable]
	public class Stamina {
		public float min;
		public float max;
		public float s;
		public bool regenerating;
		public float regenTime=0.03f;
		public float staminaPerTick = 1f;

		[HideInInspector] public PL_Controller pc;

		public IEnumerator RegenerateStamina() {
			regenerating = true;
			while (s < max) {
				yield return new WaitForSeconds(regenTime / (pc.Grounded() ? 1.3f : 1f));
				// if (pc.state != PlayerState.sliding) s++;
			}

			if (s > max) s = max;
			regenerating = false;
		}



		public void Add(float value) {
			s += value;
			if (pc.hc != null) pc.hc.UpdateStaminaBars();
		}

		public void Subtract(float value) {
			s -= value;
			if (pc.hc != null) pc.hc.UpdateStaminaBars();
		}

		// public void operator +=(float value)
		// {
		//     this.s = this.s + value;
		//     pc.hc.UpdateStaminaBars();
		// }
		// public Stamina operator -=(float value) => s -= value;

	}

	[Serializable]
	public class Dash : Force {
		[Header("Dash Exclusive")]
		public float hardCDTime = 0.1f;
		[Range(0, 100)] public float staminaPer = 30f;

		[Header("Gravity")]
		public float gravityDashForce = 50f;

		[Header("No Gravity")]
		public float noGravDashTime = 0.3f;
	}

	[Serializable]
	public class Force {
		[Header("Bools")]
		public bool can = true;
		bool[] goneBack = new bool[2];

		[Header("Force")]
		public float force = 45f;
		[Range(500, 2000)] public int decayIncrements = 1000;
		public float decaySpeed = 10f;

		[Header("Directions")]

		public Vector3 direction;
		[Range(0.01f, 0.075f)] public float directionChangeSpeed = 0.03f;

		[Header("State")]
		public MovementState state = MovementState.none;

		public void ChangeDirection(Vector3 moveDirection) {
			Vector3 moveD = new(Math.Sign(moveDirection.x), 0, Math.Sign(moveDirection.z));
			Vector3 forceD = new(Math.Sign(this.direction.x), 0, Math.Sign(this.direction.z));

			// Check if the player is moving in the opposite direction of the dash and if they are, change the dash direction to suit them

			if (-moveD.x == forceD.x && moveD.x != 0 && forceD.x != 0) {
				if (this.direction.x <= 0) this.direction = new(this.direction.x + this.directionChangeSpeed, this.direction.y, this.direction.z);
				else if (this.direction.x >= 0) this.direction = new(this.direction.x - this.directionChangeSpeed, this.direction.y, this.direction.z);
				goneBack[0] = true;
			}

			if (-moveD.z == forceD.z && moveD.z != 0 && forceD.z != 0) {
				if (this.direction.z <= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z + this.directionChangeSpeed);
				else if (this.direction.z >= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z - this.directionChangeSpeed);
				goneBack[1] = true;
			}
		}
	}

	

}


//* Legacy
// [Serializable]
//     public class Force {

//         public enum ForceState {
//             start,
//             middle,
//             end
//         }

//         public ForceState   forceState;
//         public float        force;
//         public bool         can = true;
//         public float        directionChangeSpeed;
//         public Vector3      direction;
//         public bool[]       goneBack = new bool[2];

//         // public void DirectionChange() {
//         //     Vector3 moveD = new(Math.Sign(PL_Controller.moveDirection.x), 0, Math.Sign(PL_Controller.moveDirection.z));
//         //     Vector3 forceD = new(Math.Sign(this.direction.x), 0, Math.Sign(this.direction.z));

//         //     // Check if the player is moving in the opposite direction of the dash and if they are, change the dash direction to suit them

//         //     if (-moveD.x == forceD.x && moveD.x != 0 && forceD.x != 0) {
//         //         if (this.direction.x <= 0) this.direction = new(this.direction.x + this.directionChangeSpeed, this.direction.y, this.direction.z);
//         //         else if (this.direction.x >= 0) this.direction = new(this.direction.x - this.directionChangeSpeed, this.direction.y, this.direction.z);
//         //         goneBack[0] = true;
//         //     }

//         //     if (-moveD.z == forceD.z && moveD.z != 0 && forceD.z != 0) {
//         //         if (this.direction.z <= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z + this.directionChangeSpeed);
//         //         else if (this.direction.z >= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z - this.directionChangeSpeed);
//         //         goneBack[1] = true;
//         //     }

//         //     // if (goneBack[0]) this.direction = new(Mathf.Clamp(this.direction.x, -.8f, .8f), this.direction.y, this.direction.z);
//         //     // if (goneBack[1]) this.direction = new(this.direction.x, this.direction.y, Mathf.Clamp(this.direction.z, -.8f, .8f));
//         // }

//         public void ResetGoneBack() => this.goneBack = new bool[] { false, false };

//         public void ResetDirection() => direction = Vector3.zero;
//     }