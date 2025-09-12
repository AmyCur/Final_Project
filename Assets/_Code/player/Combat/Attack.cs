using Elements;
using Magical;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : ScriptableObject {

    [Header("Main Attack")]
    public float damage;
    public float attackCD;
    public float range;
    public bool canAttack = true;
    protected bool keyDown;
    public Element element;

    [Header("Alt Attack")]
    public float altCD;
    public float altAttackCD = 5f;
    public bool canAltAttack = true;
    protected bool altKeyDown;
    RadialUpdater ru;

    [HideInInspector] public PlayerController pc;


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

    public virtual void OnClick() {
        Debug.Log("CLicked");
        keyDown = true;

        if (pc == null) {
            pc = mas.player.GetPlayer();
        }

    }
    public virtual void OnClickHold() { }

    public virtual void OnALtClick() { altKeyDown = true; }
    public virtual void OnALtRelease() { altKeyDown = false; }

    public virtual void OnRelease() {
        keyDown = false;


    }

    protected IEnumerator AttackCooldown() {
        canAttack = false;
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }
    
    protected IEnumerator AltAttackCooldown() {
        if (ru == null) {
            ru = GameObject.FindGameObjectWithTag("RadialBar").GetComponent<RadialUpdater>();
        }
        canAltAttack = false;
        const int chunks = 30;
        float altCD = altAttackCD / (float)chunks;
        for (int i = 0; i < chunks+1; i++) {
            ru.UpdateProgress(Mathf.Ceil(i * 100f/chunks));
            yield return new WaitForSeconds(altCD);
        }
        canAltAttack= true;
    }

}