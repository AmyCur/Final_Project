
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Attack", menuName = "Attacks/Create/Lighting", order = 0)]
public sealed class LightningAttack : Attack {

    List<EntityController> hitEnems = new();

    [Header("Lightning")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material damagedMaterial;

    IEnumerator CheckForEnemies() {
        while (keyDown) {
            EntityController[] ecs = hitEnemies(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, range);

            if (ecs != null) {
                foreach (EntityController ec in ecs) {
                    if (ec != null && !hitEnems.Contains(ec)) {
                        ec.GetComponent<MeshRenderer>().material = selectedMaterial;
                        hitEnems.Add(ec);
                    }
                }
            }



            yield return new WaitForSeconds(0.01f);
        }
    }

    public override void OnClick() {
        base.OnClick();

        hitEnems = new();
        pc.StartCoroutine(CheckForEnemies());
    }

    public override void OnALtClick() {
        base.OnALtClick();

        if (pc == null) pc = mas.player.GetPlayer();


        if (pc.rb != null) { pc.SV.Add(new(new(0, 20, 0), pc)); Debug.Log("Not failed"); }
        else Debug.Log("ALt failed");

        if (pc != null) pc.StartCoroutine(AltAttackCooldown());

    }



    IEnumerator SetToDefault(EntityController ec) {
        ec.GetComponent<MeshRenderer>().material = damagedMaterial;
        yield return new WaitForSeconds(.2f);
        if (!hitEnems.Contains(ec)) ec.GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public override void OnRelease() {
        base.OnRelease();

        int enemies = hitEnems.Count;
        // Peaks at 10 enemies for double damage
        float damageMultiplier = Mathf.Clamp(1 + (Mathf.Pow(enemies, 2) / 100), 1, 2);

        foreach (EntityController ec in hitEnems) {
            if (ec != null) {
                pc.StartCoroutine(SetToDefault(ec));
                ec.TakeDamage(damage * damageMultiplier, element);
            }

        }

        pc.StartCoroutine(AttackCooldown());
    }

    public override void OnClickHold() {

    }
}