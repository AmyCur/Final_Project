
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Attack", menuName = "Attacks/Create/Lighting", order = 0)]
public class LightningAttack : Attack {

    List<EnemyController> hitEnemies = new();

    IEnumerator CheckForEnemies() {
        while (keyDown) {
            EnemyController ec = hitEnemy(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, range);

            if (ec != null && !hitEnemies.Contains(ec)) {
                Debug.Log($"Adding {ec.name}");
                hitEnemies.Add(ec);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    public override void OnClick() {
        base.OnClick();

        hitEnemies = new();
        pc.StartCoroutine(CheckForEnemies());
    }

    public override void OnRelease() {
        base.OnRelease();

        int enemies = hitEnemies.Count;
        // Peaks at 10 enemies for double damage
        float damageMultiplier = Mathf.Clamp(1 + (Mathf.Pow(enemies, 2) / 100), 1, 2);

        foreach (EnemyController ec in hitEnemies) {
            ec.TakeDamage(damage * damageMultiplier);
        }

        pc.StartCoroutine(AttackCooldown());
    }

    public override void OnClickHold() {

    }
}