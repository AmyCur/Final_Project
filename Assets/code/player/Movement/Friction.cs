using UnityEngine;

namespace Player.Movement{
	public static class Friction{
		public static void SetFriction(this Collider col, float friction){
			col.material.dynamicFriction=friction;
		}
	}
}