using MathsAndSome;
using UnityEngine;
using System.Collections;   
using Magical;

namespace Combat.Attacks{
    [CreateAssetMenu(fileName = "New Homijng Attack", menuName = "Attacks/Other/Create Homing Attack")]
    public class HomingAttack : AlternateAttack
    {
        
        public float HomeTime=5f;
        CombatController cc => pc.GetComponent<CombatController>();


        
        public override bool keyDown() => magic.key.down(keys.ability);
		
        public override bool keyStayDown() => magic.key.gk(keys.ability);
		
        public override bool keyUp() => magic.key.up(keys.ability);
        
        public IEnumerator OverrideEnemies()
        {
            while (cc.homing)
            {
                foreach(GameObject enm in Entity.Entity.GetAllEnemies()){
                    enm.GetComponent<SphereCollider>().enabled = true;
                    yield return 0;
                }
                
                yield return 0;
            }
        }

        public IEnumerator HomeTimer(){
            cc.homing=true;
            pc.StartCoroutine(OverrideEnemies());
            yield return new WaitForSeconds(HomeTime);
            cc.homing=false;
        }

        public override void OnClick(){
            pc.StartCoroutine(HomeTimer());
            base.OnClick();
        }
    }
}
