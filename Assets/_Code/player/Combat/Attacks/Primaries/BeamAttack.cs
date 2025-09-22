using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Beam Attack", menuName = "Attacks/Primary/Beam", order = 0)]
public class BeamAttack : PrimaryAttack {

    [SerializeField] List<EntityController> hitEnems = new();



    [Header("Lightning")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material damagedMaterial;

    IEnumerator CheckForEnemies() {
        while (keyStayDown()) {
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

    IEnumerator SetToDefault(EntityController ec) {
        ec.GetComponent<MeshRenderer>().material = damagedMaterial;
        yield return new WaitForSeconds(.2f);
        if (!hitEnems.Contains(ec) && ec != null) ec.GetComponent<MeshRenderer>().material = defaultMaterial;
    }



    public override void OnClick() {
        hitEnems = new();
        pc.StartCoroutine(CheckForEnemies());
    }

    public override void OnRelease() {
        int enemies = hitEnems.Count;
        // Peaks at 10 enemies for double damage
        float damageMultiplier = Mathf.Clamp(1 + (Mathf.Pow(enemies, 2) / 100), 1, 2);

        foreach (EntityController ec in hitEnems) {
            pc.StartCoroutine(SetToDefault(ec));
            ec.TakeDamage(damage * damageMultiplier, element);

        }

        pc.StartCoroutine(AttackCooldown());
    }


}