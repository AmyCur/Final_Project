using Elements;
using EntityLib;
using Magical;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class thingie : ScriptableObject{

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

    protected ENT_Controller[] hitEnemies(Vector3 startPos, Vector3 direction, float range) {
        Debug.DrawLine(startPos, startPos + (direction * range), Color.red, 1);
        List<ENT_Controller> ecs = new();
        Vector3 pos = startPos;
        float dist = range;

        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, range);

        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.isEntity(typeof(ENM_Controller))) {
                    ENT_Controller ec = hit.collider.GetComponent<ENT_Controller>();

                    if (!!ec) {
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

    public Player.PL_Controller pc => mas.player.GetPlayer();

    public abstract bool keyDown();
    public abstract bool keyStayDown();


    public abstract void OnClick();
    public abstract void OnRelease();
}
