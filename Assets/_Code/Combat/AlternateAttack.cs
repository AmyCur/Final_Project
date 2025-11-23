using Magical;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// using System;
// using UnityEditor;
// using System.Reflection;
using EntityLib;
using UnityEngine.UI;

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
						string newV = v;
						bool negate = newV.Contains("!");
						if (negate) newV = v.Split("!")[1];
						bool toAdd = ((bool) (altAttack.GetType().GetProperty(newV).GetValue(altAttack, null)));
						toAdd = negate ? !toAdd : toAdd;
						bools.Add(toAdd);
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

			[Header("UI")]

			public Sprite icon;

			[Header("Vortex")]

			public float vortexRadius;
			public float vortexForce;
			public float vortexLifetime;
			public float vortexDamage;
			public Vortex.Pullable pullable;
			public Vortex.Polarity polarity;


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
			Vector2 defaultMoveSpeed = Vector2.zero;
			Vector2 launchMoveSpeed = Vector2.zero;

			[Header("Slamming")]

			public float slamForce = 10_000;

			public override bool keyDown() => magic.key.down(keys.assist);
			public override bool keyStayDown() => magic.key.gk(keys.assist);
			public override bool keyUp() => magic.key.up(keys.assist);

			void CreateDamageSphere(float dmg) {
				Collider[] ec = Physics.OverlapSphere(pc.transform.position, vortexRadius);

				foreach (Collider col in ec) {
					Entity.ENT_Controller RB_Controller = col.GetComponent<Entity.ENT_Controller>();
					if (!!RB_Controller && !RB_Controller.isEntity(typeof(Player.PL_Controller))) {
						RB_Controller.TakeDamage(dmg, element);
					}
				}
			}

			IEnumerator FunctionVortex() {
				CreateDamageSphere(vortexDamage);
				for (float i = 0; i < vortexLifetime; i += 0.01f) {
					Collider[] cols = Physics.OverlapSphere(pc.transform.position, vortexRadius);

					foreach (Collider col in cols) {
						if (col.isEntity(typeof(ENM_Controller))) {
							// Entity.ENT_Controller c = col.GetComponent<Entity.ENT_Controller>();

							if (col.TryGetComponent<Entity.ENT_Controller>(out Entity.ENT_Controller c)) {
								Vector3 dir = (col.transform.position - pc.transform.position).normalized;
								float f = polarity == Vortex.Polarity.inwards ? -vortexForce : vortexForce;

								switch (pullable) {
									case Vortex.Pullable.enemy:
										if (c is ENM_Controller enemC)
											enemC.SV.Add(new(dir * f, enemC));
										break;
									case Vortex.Pullable.player:
										if (c is Player.PL_Controller pc)
											pc.SV.Add(new(dir * f, pc));
										break;
									case Vortex.Pullable.both:
										(c as RB_Controller).SV.Add(new(dir * f, c as RB_Controller));
										break;
								}

							}

							else {
								Debug.LogError($"{col.name} is an enemy thats missing a Entity.ENT_Controller!");
								break;
							}


						}
					}

					yield return new WaitForSeconds(0.01f);
				}

			}

			public void HandleVortex() => pc.StartCoroutine(FunctionVortex());

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

			IEnumerator SetLaunchMoveSpeed() {
				pc.forwardSpeed = launchMoveSpeed.y;
				pc.sidewaysSpeed = launchMoveSpeed.x;

				yield return new WaitUntil(() => pc.Grounded());

				pc.forwardSpeed = defaultMoveSpeed.y;
				pc.sidewaysSpeed = defaultMoveSpeed.x;
			}

			public void HandleLaunch() {
				pc.rb.AddForce(launchForce);
				pc.StartCoroutine(SetLaunchMoveSpeed());
			}

			public void HandleSlam() {
				pc.rb.AddForce(0, -slamForce * Time.deltaTime, 0);
			}

			void Start() {
				if (defaultMoveSpeed == Vector2.zero) defaultMoveSpeed = new Vector2(pc.sidewaysSpeed, pc.forwardSpeed);
				if (launchMoveSpeed == Vector2.zero) launchMoveSpeed = new Vector2(defaultMoveSpeed.x / 3, defaultMoveSpeed.y / 3);
			}


			public override void OnClick() {
				bool attackFailed = false;
				Start();
				List<AttackAbilities> alts = new();
				if (alternateConditions.Length > 0) alts = shouldAlt ? alternateAbilities : attackAbilities;
				else alts = attackAbilities;
				foreach (AttackAbilities ab in alts) {
					switch (ab) {
						case (AttackAbilities.vortex):
							HandleVortex();
							break;
						case (AttackAbilities.heal):
							if (alts.Count > 1 || pc.health.h != 100)
								HandleHeal();
							else attackFailed = true;
							break;
						case (AttackAbilities.launch):
							HandleLaunch();
							break;
						case (AttackAbilities.slam):
							HandleSlam();
							break;
					}
				}
				if (!attackFailed) base.OnClick();
			}

			[HideInInspector] public int cooldownProgress;
			[HideInInspector] public int attackCDIncrements = 100;

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
		// 	AlternateAttack at;
		// 	SerializedProperty altCon;
		//
		// 	void OnEnable() {
		// 		at = target as AlternateAttack;
		// 	}
		// 	bool altConditions;
		//
		// 	  public Sprite sprite;
		// 	public override void OnInspectorGUI() {
		// 		EditorGUI.BeginChangeCheck();
		// 		if (at.alternateConditions.Length > 0) {
		// 			// EditorGUILayout.PropertyField(altCon, GUIContent.none);
		// 			// at.alternateAbilities = EditorGUILayout.ObjectField(at.alternateAbilities, typeof(List<AlternateAttack>), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as List<AlternateAttack>;
		// 			//   sprite = EditorGUILayout.ObjectField(sprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
		// 			// at.alternateAbilities = (AlternateAttack)
		// 			// 	EditorGUILayout.EnumPopup(at.alternateAbilities);
		// 		}
		// 		EditorGUI.EndChangeCheck();
		//
		// 		base.OnInspectorGUI();
		// 		serializedObject.ApplyModifiedProperties();
		// 	}
		// }
	}
}