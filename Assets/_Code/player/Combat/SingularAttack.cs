using Elements;
using Magical;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingularAttack : ScriptableObject{

    public enum AttackType {
        singular,
        hold
    }

    [Header("Stats")]
    public float damage;
    public float attackCD;
    public float range;
    public bool canAttack = true;
    public Element element;
    public AttackType attackType;

    protected EntityController[] hitEnemies(Vector3 startPos, Vector3 direction, float range) {
        Debug.DrawLine(startPos, startPos + (direction * range), Color.red, 1);
        List<EntityController> ecs = new();
        Vector3 pos = startPos;
        float dist = range;

        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, range);

        if (hits.Length > 0) {
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

    protected IEnumerator AttackCooldown() {
        canAttack = false;
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }

    public PlayerController pc => mas.player.GetPlayer();

    public abstract bool keyDown();
    public abstract bool keyStayDown();


    public abstract void OnClick();
    public abstract void OnRelease();
}
