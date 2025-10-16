using Elements;
using EntityLib;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingularAttack : ScriptableObject {

    protected AudioSource source;

    protected ENT_Controller[] hitEnemies(Vector3 startPos, Vector3 direction, float range) {
        Debug.DrawLine(startPos, startPos + (direction * range), Color.red, 1);
        List<ENT_Controller> ecs = new();
        Vector3 pos = startPos;
        float dist = range;

        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, range);


        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.isEnemy()) {
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


    protected Player.PL_Controller pc => mas.player.GetPlayer();

    public enum AttackType {
        single,
        hold
    }

    public AttackType attackType;
    public float damage;
    public float attackCD;
    public float range;
    public bool canAttack = true;
    

    public Element element;

    public AudioClip onClickClip;
    public AudioClip onReleaseClip;
    public AudioClip onDamageClip;

    protected void PlayClip(AudioClip clip) { if (!!clip) source.PlayOneShot(clip); }

    public abstract bool keyDown();
    public abstract bool keyUp();
    public abstract bool keyStayDown();

    public virtual void OnClick() { }
    public virtual void OnRelease() { }

    protected IEnumerator AttackCooldown() {
        canAttack = false;
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }
}