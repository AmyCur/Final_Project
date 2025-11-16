using EntityLib;
using MathsAndSome;
using UnityEngine;

public class ColliderController : MonoBehaviour {

    BeamAttack atk;

    void OnTriggerEnter(Collider other) {
        
        if (other.isEntity(typeof(ENM_Controller))) {
            // atk.AddEnemy(other.GetComponent<Entity.ENT_Controller>());
        }
    }

	void Awake() {
        Combat.CombatController cc = mas.player.GetPlayerObj().GetComponent<Combat.CombatController>();

        cc.attacks.ForEach((attack) => {
            if (attack.primary.GetType() == typeof(BeamAttack)) atk = attack.primary as BeamAttack;
        });
	}
}
