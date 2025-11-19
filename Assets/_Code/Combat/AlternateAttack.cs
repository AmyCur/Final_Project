using Magical;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
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

		public class AlternateAttack : SingularAttack {

			CombatController cc;

			[Header("Abilities")]

			public List<AttackAbilities> attackAbilities;

			// These are used if the alternate condition is met
			public List<AttackAbilities> alternateAbilities;

			[SerializeField] string[] alternateConditions;
			bool shouldAlt => AltReflection.GetAttackProperties(alternateConditions, this);
			public bool grounded => pc.Grounded();

			[Header("Vortex")]

			public float vortexRadius;

			[Header("Healing")]

			[SerializeField] HealingType healingType;
			[SerializeField] float healthPerTick = 100f;
			[SerializeField] float timeBetweenTick = 0.3f;

			// Use either ticks or healTime
			bool useTicks;
			[SerializeField] int ticks = 10;
			[SerializeField] float healTime = 5f;

			enum HealingType {
				instant,
				overtime
			}


			public override bool keyDown() => magic.key.down(keys.assist);
			public override bool keyStayDown() => magic.key.gk(keys.assist);
			public override bool keyUp() => magic.key.up(keys.assist);

			public void HandleVortex() { }
			public void HandleHeal() { }
			public void HandleLaunch() { }
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
	}
}