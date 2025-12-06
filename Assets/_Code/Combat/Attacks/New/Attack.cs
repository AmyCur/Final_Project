using UnityEngine;
using UnityEditor;

namespace Combat.Attack {
	enum AttackMode {
		Single,
		Hold
	}



	public class Attack : ScriptableObject {
		public float damage = 10f;
		// AttackMode attackMode = AttackMode.Single;
		public float attackCD = 0.5f;
	}

	// [CanEditMultipleObjects]
	// [CustomEditor(typeof(Attack))]
	// public class AttackInspector : Editor {
	// 	Attack atk;
	// 	void OnEnable() {
	// 		atk = target as Attack;
	// 	}
	// }
}
