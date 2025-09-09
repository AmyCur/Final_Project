
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Attack", menuName = "Attacks/Create/Lighting", order = 0)]
public sealed class LightningAttack : Attack {

    List<EntityController> hitEnemies = new();

    [Header("Lightning")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material damagedMaterial;

    IEnumerator CheckForEnemies() {
        while (keyDown) {
            EntityController ec = hitEnemy(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, range);

            if (ec != null && !hitEnemies.Contains(ec)) {
                ec.GetComponent<MeshRenderer>().material = selectedMaterial;
                hitEnemies.Add(ec);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public override void OnClick() {
        base.OnClick();

        hitEnemies = new();
        pc.StartCoroutine(CheckForEnemies());
    }


    IEnumerator SetToDefault(EntityController ec) {
        ec.GetComponent<MeshRenderer>().material = damagedMaterial;
        yield return new WaitForSeconds(.2f);
        if (!hitEnemies.Contains(ec)) ec.GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public override void OnRelease() {
        base.OnRelease();

        int enemies = hitEnemies.Count;
        // Peaks at 10 enemies for double damage
        float damageMultiplier = Mathf.Clamp(1 + (Mathf.Pow(enemies, 2) / 100), 1, 2);

        foreach (EntityController ec in hitEnemies) {
            pc.StartCoroutine(SetToDefault(ec));
            ec.TakeDamage(damage * damageMultiplier, element);
            
        }

        pc.StartCoroutine(AttackCooldown());
    }

    public override void OnClickHold() {

    }
}