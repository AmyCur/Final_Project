using Globals;
using MathsAndSome;
using System.Collections;
using UnityEngine;
using Vortex;

namespace Vortex {
    public static class Create {
        public static GameObject CreateVortex(GameObject vortex, Vector3 position) {
            return GameObject.Instantiate(vortex, position, Quaternion.identity, mas.get.VortexHolder());
        }
    }
    
    public enum Pullable {
        enemy,
        player,
        both
    }

    public enum Polarity {
        inwards,
        outwards
    }
}

public class VortexController : MonoBehaviour {

        public Pullable pullable;
        public Polarity polarity;

        public float radius = 1f;
        public float force = 10f;
        public float vortexLifetime = 3f;

        IEnumerator DestroyVortex() {
            yield return new WaitForSeconds(vortexLifetime);
            Destroy(gameObject);
        }
        void Awake() {
            StartCoroutine(DestroyVortex());
        }

        void Update() {
            Collider[] cols = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider col in cols) {
                // if (glob.isEntity(col.tag)) {
                    Entity.ENT_Controller c = col.GetComponent<Entity.ENT_Controller>();

                    if (!c) {
                        Debug.LogError($"{col.name} is an enemy thats missing a Entity.ENT_Controller!");
                        break;
                    }


                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    float f = polarity == Polarity.inwards ? -force : force;

                    switch (pullable) {
                        case Pullable.enemy:
                            if (c is ENM_Controller enemC)
                                enemC.SV.Add(new(dir * f, enemC));
                            break;
                        case Pullable.player:
                            if (c is Player.PL_Controller pc)
                                pc.SV.Add(new(dir * f, pc));
                            break;
                        case Pullable.both:
                            (c as RB_Controller).SV.Add(new(dir * f, c as RB_Controller));
                            break;
                    }
                }
            // }
        }

        void OnDrawGizmos() {
            if (Cur.Settings.Combat.drawVortexRadius)
                Gizmos.DrawWireSphere(transform.position, radius);
        }
    }