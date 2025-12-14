using UnityEngine;
using static EntityLib.Entity;
using static Cur.Settings.Combat;
using MathsAndSome;


public sealed class MeleeEnemy : ENM_Controller
{
    public override bool shouldHunt()
    {
        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxHuntRange)) return hit.isEntity() && hit.distance < maxHuntRange && canHunt;


        return false;
    }
    public override bool shouldSeek()
    {
       

        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxSeekRange)) return hit.isEntity() && hit.distance < maxSeekRange && canSeek;
        return false;
    }
    
    public override bool shouldAttack()
    {
        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxSeekRange)) return hit.isEntity() && hit.distance < maxAttackRange && canAttack;
        return false;
    }

    public override void Hunt() {
        agent.SetDestination(playerPosition);
    }
    public override void Seek() { }

    public override void Update()
    {
        base.Update();
    }

    bool attackSuccessful()
    {
        Vector3 rbV = pc.rb.linearVelocity;
        rbV = new Vector3(Mathf.Abs(rbV.x), Mathf.Abs(rbV.y), Mathf.Abs(rbV.z));
        float mag = (rbV.x > rbV.z ? rbV.x : rbV.z) + rbV.y;
        Debug.Log(mag);

        if (mag >= EnemyGlobals.Melee.velocityFailureSpeed)
        {
            // Exlusive
            int randint = new System.Random().Next(2);
            return randint == 0;
        }

        return true;
    }

    public override void Attack()
    {
        if (canAttack)
        {
            if (Physics.Raycast(pos, direction, attackData.attackRange))
            {
                if (attackSuccessful()) pc.TakeDamage(attackData.damage, attackElement);
                StartCoroutine(CooldownAttack());
            }
        }

    }
}    