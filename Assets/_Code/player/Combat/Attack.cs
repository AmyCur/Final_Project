using Elements;
using Magical;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : ScriptableObject{

    public float damage;
    public float attackCD;
    public float range;
    public bool canAttack = true;
    public PlayerController pc;
    protected bool keyDown;
    public Element element;

    protected EntityController[] hitEnemies(Vector3 startPos, Vector3 direction, float range) {
        Debug.DrawLine(startPos, startPos + (direction * range), Color.red, 1);
        List<EntityController> ecs = new();
        Vector3 pos = startPos;
        float dist = range;

        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, range);
        

        if (hits.Length>0) {
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Enemy") {
                    EntityController ec = hit.collider.GetComponent<EntityController>();

                    if (ec != null) {
                        ecs.Add(ec);
                    }
                }
            }
        }

        if (ecs.Count > 0) {
            return ecs.ToArray();
        }
       
        return null;
    }

    public virtual void OnClick() {
        Debug.Log("CLicked");
        keyDown = true;

        if (pc == null) {
            pc = mas.player.GetPlayer();
        }

    }
    public virtual void OnClickHold() { }
    public virtual void OnRelease() { 
        keyDown = false;


    }

    protected IEnumerator AttackCooldown() {
        canAttack = false;
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }

}