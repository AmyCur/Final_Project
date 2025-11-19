using Magical;
using UnityEngine;
using System.Collections;

namespace Combat{
	namespace Attacks{
		public enum AltAttackType
		{
			assist,
			ability
		}

		public class AlternateAttack : SingularAttack {

			CombatController cc;


			public AltAttackType altAttackType;
			KeyCode[] useKey => altAttackType == AltAttackType.assist ? keys.assist : keys.ability;
			
			public override bool keyDown() => magic.key.down(useKey);
			public override bool keyStayDown() => magic.key.gk(useKey);
			public override bool keyUp() => magic.key.up(useKey);

			public int cooldownProgress;
			public int attackCDIncrements = 20;

			public override IEnumerator AttackCooldown() {

				cc ??= pc.GetComponent<CombatController>();

				canAttack = false;
				for (cooldownProgress = 0; cooldownProgress < attackCDIncrements + 1; cooldownProgress++) {
					yield return new WaitForSeconds(attackCD / (float) attackCDIncrements);
					
					if(cc.attacks[cc.caIndex].assist == this) pc.hc.assistBar.UpdateBarCD((float)cooldownProgress, attackCDIncrements);
					if(cc.attacks[cc.caIndex].ability == this) pc.hc.abilityBar.UpdateBarCD((float)cooldownProgress, attackCDIncrements);
				}
				canAttack = true;
			}
		}
	}
}