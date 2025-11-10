using UnityEngine;
using static EntityLib.Entity;
using static GameDebug.Combat;
using MathsAndSome;
using System.Collections;

public class RangedEnemy : ENM_Controller
{

    Vector3 pos => transform.position;
    Vector3 direction => (playerPosition - pos).normalized;


    public enum MovementChoice
    {
        to_player,
        walk_random,
        retreat
    }

    MovementChoice[] MovementChoices = { MovementChoice.to_player, MovementChoice.walk_random, MovementChoice.retreat };
    MovementChoice choice = MovementChoice.to_player;

    [Header("Hunting Choices")]
    [SerializeField] protected MovementChoice rc=MovementChoice.to_player;
    [SerializeField] protected float movementChoiceTime=5f;
    [SerializeField] protected Vector2[] Offset = new Vector2[2];
    protected bool canChangeHuntChoice=true;


    protected IEnumerator MovementChoiceCD()
    {
        canChangeHuntChoice = false;
        yield return new WaitForSeconds(movementChoiceTime);
        canChangeHuntChoice = true;
    }

    Vector3 RandomOffset()
    {
        return new System.Random().NextDouble()
    }


    public override void Seek() { }
    public override void Hunt() {
        if (canChangeHuntChoice)
        {
            choice = MovementChoices[new System.Random().Next(3)];
        }

        Vector3 destination = choice switch
        {
            MovementChoice.to_player => playerPosition+,
            MovementChoice.walk_random => playerPosition          
        };
    }
    public override void Attack() { }

    public override bool shouldHunt()
    {
        if (drawHuntRay)
        {
            Debug.DrawLine(pos, pos + (direction * minHuntRange) + (direction * (maxHuntRange - minHuntRange)), Color.red);
            Debug.DrawLine(pos, pos + (direction * minHuntRange), Color.yellow);
        }
        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxHuntRange)) return hit.isEntity() && hit.distance.inRange(minHuntRange, maxHuntRange) && canHunt;
        return false;
    }
    public override bool shouldSeek()
    {
        if (drawSeekRay)
        {
            Debug.DrawLine(pos, pos + (direction * minSeekRange) + (direction * (maxSeekRange - minSeekRange)), Color.green);
            Debug.DrawLine(pos, pos + (direction * minSeekRange), Color.blue);
        }

        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxSeekRange)) return hit.isEntity() && hit.distance.inRange(minSeekRange, maxSeekRange) && canSeek;
        return false;
    }

    public override bool shouldAttack()
    {
        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxSeekRange)) return hit.isEntity() && hit.distance.inRange(minAttackRange, maxAttackRange) && canAttack;
        return false;
    }
}