using Magical;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Reflection;

namespace Combat {
	namespace Attacks {
		public enum AltAttackType {
			assist,
			ability
		}

		public enum AttackAbilities {
			vortex,
			launch,
			heal,
			slam
		}

		public static class AltReflection {
			public static bool GetAttackProperties(string[] checkValue, AlternateAttack altAttack) {
				List<bool> bools = new();
				foreach (string v in checkValue) {
					try {
						bools.Add((bool) (altAttack.GetType().GetProperty(v).GetValue(altAttack, null)));
					}
					catch { }
				}

				foreach (bool b in bools) {
					if (!b) return false;
				}

				return true;
			}

		}

		[CreateAssetMenu(fileName = "New alternate attack", menuName = "Attacks/Create/Alternate")]
		public class AlternateAttack : SingularAttack {

			CombatController cc;

			[Header("Abilities")]

			public List<AttackAbilities> attackAbilities;

			// These are used if the alternate condition is met
			public List<AttackAbilities> alternateAbilities;

			public string[] alternateConditions;
			bool shouldAlt => AltReflection.GetAttackProperties(alternateConditions, this);
			public bool grounded => pc.Grounded();

			[Header("Vortex")]

			public float vortexRadius;

			[Header("Healing")]

			public HealingType healingType;
			public float healingAmount = 100f;

			public float healthPerTick = 100f;
			public float timeBetweenTick = 0.3f;

			// Use either ticks or healTime
			bool useTicks;
			public int ticks = 10;
			public float healTime = 5f;

			public enum HealingType {
				instant,
				overtime
			}

			[Header("Launching")]

			public Vector3 launchForce;

			public override bool keyDown() => magic.key.down(keys.assist);
			public override bool keyStayDown() => magic.key.gk(keys.assist);
			public override bool keyUp() => magic.key.up(keys.assist);

			public void HandleVortex() { }

			public IEnumerator HealOverTime() {

				for (int i = 0; i < ticks; i++) {
					pc.health.h += healthPerTick;
					yield return new WaitForSeconds(timeBetweenTick);
				}
			}

			public void HandleHeal() {
				if (healingType == HealingType.instant) pc.health.h += healingAmount;
				else pc.StartCoroutine(HealOverTime());
			}

			public void HandleLaunch() { pc.rb.AddForce(launchForce); }
			public void HandleSlam() { }


			public override void OnClick() {
				foreach (AttackAbilities ab in shouldAlt ? alternateAbilities : attackAbilities) {
					switch (ab) {
						case (AttackAbilities.vortex):
							HandleVortex();
							break;
						case (AttackAbilities.heal):
							HandleHeal();
							break;
						case (AttackAbilities.launch):
							HandleLaunch();
							break;
						case (AttackAbilities.slam):
							HandleSlam();
							break;
					}
				}
			}

			[HideInInspector] public int cooldownProgress;
			[HideInInspector] public int attackCDIncrements = 20;

			public override IEnumerator AttackCooldown() {

				cc ??= pc.GetComponent<CombatController>();

				canAttack = false;
				for (cooldownProgress = 0; cooldownProgress < attackCDIncrements + 1; cooldownProgress++) {
					yield return new WaitForSeconds(attackCD / (float) attackCDIncrements);

					if (cc.attacks[cc.caIndex].assist == this) pc.hc.assistBar.UpdateBarCD((float) cooldownProgress, attackCDIncrements);
					if (cc.attacks[cc.caIndex].ability == this) pc.hc.abilityBar.UpdateBarCD((float) cooldownProgress, attackCDIncrements);
				}
				canAttack = true;
			}
		}

		// [CanEditMultipleObjects]
		// [CustomEditor(typeof(AlternateAttack))]
		// public class AltAttackInspector : Editor {
		//
		// 	public void DisplayList(SerializedProperty item, ref bool Foldout, string name, string itemName = "item") {
		// 		GUILayout.Label(name);
		//
		// 		item.arraySize = EditorGUILayout.IntField("Size", item.arraySize);
		// 		Foldout = EditorGUILayout.Foldout(Foldout, "items");
		//
		// 		if (Foldout) {
		//
		// 			for (int i = 0; i < item.arraySize; i++) {
		// 				EditorGUI.indentLevel++;
		//
		// 				var dialogue = item.GetArrayElementAtIndex(i);
		// 				EditorGUILayout.PropertyField(dialogue, new GUIContent($"{itemName}  {i}"));
		//
		// 				EditorGUI.indentLevel--;
		// 			}
		// 		}
		// 	}
		//
		// 	AlternateAttack at;
		// 	SerializedProperty altCon;
		//
		// 	void OnEnable() {
		// 		at = target as AlternateAttack;
		// 	}
		// 	bool altConditions;
		//
		// 	public override void OnInspectorGUI() {
		// 		EditorGUI.BeginChangeCheck();
		// 		if (at.alternateConditions.Length > 0) {
		// 			EditorGUILayout.PropertyField(altCon);
		//
		// 			// at.alternateAbilities = (AlternateAttack)
		// 			// 	EditorGUILayout.EnumPopup(at.alternateAbilities);
		// 		}
		// 		EditorGUI.EndChangeCheck();
		// 	}
		// }
	}
}