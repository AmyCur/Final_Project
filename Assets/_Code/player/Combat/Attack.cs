using Magical;
using System.Collections;
using UnityEngine;

public abstract class Attack : ScriptableObject{

    public float damage;
    public float attackCD;
    public float range;
    public bool canAttack = true;
    protected PlayerController pc;
    protected bool keyDown;

    protected EnemyController hitEnemy(Vector3 startPos, Vector3 direction, float range) {
        Debug.DrawLine(startPos, startPos+(direction*range), Color.red, 1);
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, range)) {
            if (hit.collider.tag == "Enemy") {
                EnemyController ec = hit.collider.GetComponent<EnemyController>();
                if (ec != null) {
                    return ec;
                }
            }
        }
        
        return null;
    }

    public virtual void OnClick() {
        Debug.Log("CLicked");
        keyDown = true;

        if (pc == null) {
            pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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