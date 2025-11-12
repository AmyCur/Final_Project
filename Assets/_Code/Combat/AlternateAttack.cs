using Magical;
using UnityEngine;
using System.Collections;

public class AlternateAttack : SingularAttack {
	public override bool keyDown() => magic.key.down(keys.altAttack);
	public override bool keyStayDown() => magic.key.gk(keys.altAttack);
	public override bool keyUp() => magic.key.up(keys.altAttack);

	public int cooldownProgress;
	public int attackCDIncrements = 20;

	public override IEnumerator AttackCooldown() {
		canAttack = false;
		for (cooldownProgress = 0; cooldownProgress < attackCDIncrements + 1; cooldownProgress++) {
			yield return new WaitForSeconds(attackCD / (float) attackCDIncrements);
			pc.hc.UpdateAltCD();
		}
		canAttack = true;
	}

}