using UnityEngine;
using static EntityLib.Entity;
using static GameDebug.Combat;
using MathsAndSome;

public sealed class MeleeEnemy : EnemyController
{
    Vector3 pos => transform.position;
    Vector3 direction => (playerPosition-pos).normalized;

    public override bool shouldHunt()
    {
        if (drawHuntRay)
        {
            Debug.DrawLine(pos, pos+(direction * minHuntRange)+(direction * (maxHuntRange-minHuntRange)), Color.red);
            Debug.DrawLine(pos, pos+(direction * minHuntRange), Color.yellow);
        }
        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxHuntRange)) return hit.isEntity() && hit.distance.inRange(minHuntRange, maxHuntRange) && canHunt;
        return false;
    }
    public override bool shouldSeek()
    {
        if (drawSeekRay)
        {
            Debug.DrawLine(pos, pos+(direction * minSeekRange)+(direction * (maxSeekRange-minSeekRange)), Color.green);
            Debug.DrawLine(pos, pos+(direction * minSeekRange), Color.blue);
        }

        
        if (Physics.Raycast(pos, direction, out RaycastHit hit, maxSeekRange)) return hit.isEntity() && hit.distance.inRange(minSeekRange, maxSeekRange) && canSeek;
        return false;
    }

    public override void Hunt() { }
    public override void Seek() { }
}