using Globals;
using System.Collections;
using UnityEngine;

public class VortexController : MonoBehaviour {
    public enum Pullable {
        enemy,
        player,
        both
    }

    public Pullable pullable;

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
                Debug.LogWarning(dir * -force);

                switch (pullable) {
                    case Pullable.enemy:
                        if (ec is EnemyController enemC)
                            enemC.SV.Add(new(dir* -force, enemC));
                        break;
                    case Pullable.player:
                        if (ec is PlayerController pc)
                            pc.SV.Add(new(dir * -force, pc));
                        break;
                    case Pullable.both:
                        ec.SV.Add(new(dir * -force, ec));
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
