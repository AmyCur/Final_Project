using Globals;
using MathsAndSome;
using System.Collections;
using UnityEngine;
using Vortex;

namespace Vortex {
    public static class Create {
        public static GameObject CreateVortex(GameObject vortex, Transform transform) {
            return GameObject.Instantiate(vortex, transform.position, Quaternion.identity, mas.get.VortexHolder());
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
                if (glob.isEntity(col.tag)) {
                    EntityController ec = col.GetComponent<EntityController>();

                    if (ec == null) {
                        Debug.LogError($"{col.name} is an enemy thats missing an EntityController!");
                        break;
                    }


                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    float f = polarity == Polarity.inwards ? -force : force;

                    switch (pullable) {
                        case Pullable.enemy:
                            if (ec is EnemyController enemC)
                                enemC.SV.Add(new(dir * f, enemC));
                            break;
                        case Pullable.player:
                            if (ec is PlayerController pc)
                                pc.SV.Add(new(dir * f, pc));
                            break;
                        case Pullable.both:
                            ec.SV.Add(new(dir * f, ec));
                            break;
                    }
                }
            }
        }

        void OnDrawGizmos() {
            if (GameDebug.Combat.drawVortexRadius)
                Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

