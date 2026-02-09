using Magical;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// using System;
// using UnityEditor;
// using System.Reflection;
using Entity;
using UnityEngine.UI;

namespace Combat{
    namespace Attacks
    {
        public enum AltAttackType
        {
            assist,
            ability
        }

        [CreateAssetMenu(fileName = "New alternate attack", menuName = "Attacks/Create/Alternate")]
        public class AlternateAttack : SingularAttack
        {

            CombatController cc;

            [Header("Abilities")]

            public bool useOnFullHealth = true;

  
            public bool grounded => Instance<Player.Movement.PL_Controller>.instance.Grounded();

            [Header("UI")]

            public Sprite icon;


            public override bool keyDown() => magic.key.down(keys.assist);
            public override bool keyStayDown() => magic.key.gk(keys.assist);
            public override bool keyUp() => magic.key.up(keys.assist);

          

            void Start()
            {
                // if (defaultMoveSpeed == Vector2.zero) defaultMoveSpeed = new Vector2(pc.sidewaysSpeed, pc.forwardSpeed);
                // if (launchMoveSpeed == Vector2.zero) launchMoveSpeed = new Vector2(defaultMoveSpeed.x / 3, defaultMoveSpeed.y / 3);
            }


            public override void OnClick()
            {
                bool attackFailed = false;
                Start();
                if (!attackFailed) base.OnClick();
            }

            [HideInInspector] public int cooldownProgress;
            [HideInInspector] public int attackCDIncrements = 100;

            public override IEnumerator AttackCooldown()
            {

                cc ??= pc.GetComponent<CombatController>();

                canAttack = false;
                for (cooldownProgress = 0; cooldownProgress < attackCDIncrements + 1; cooldownProgress++)
                {
                    yield return new WaitForSeconds(attackCD / (float)attackCDIncrements);

                    if (cc.attacks[cc.caIndex].assist == this) pc.hc.assistBar.UpdateBarCD((float)cooldownProgress, attackCDIncrements);
                    else if (cc.attacks[cc.caIndex].ability == this) pc.hc.abilityBar.UpdateBarCD((float)cooldownProgress, attackCDIncrements);
                }
                canAttack = true;
            }
        }
		
    }
}