using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Beam Attack", menuName = "Attacks/Primary/Beam", order = 0)]
public class BeamAttack : PrimaryAttack {

    [SerializeField] List<EntityController> hitEnems = new();
    GameObject markerPrefab;
    List<GameObject> markers;




    [Header("Lightning")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material damagedMaterial;

    IEnumerator CheckForEnemies() {
        if (!markerPrefab) markerPrefab = Resources.Load<GameObject>("Prefabs/Combat/Marker/Marker");

        while (keyStayDown()) {
            EntityController[] cs = hitEnemies(pc.playerCamera.transform.position, pc.playerCamera.transform.forward, range);

            if (cs != null) {
                foreach (EntityController c in cs) {
                    if (c != null && !hitEnems.Contains(c)) {
                        c.GetComponent<MeshRenderer>().material = selectedMaterial;
                        hitEnems.Add(c);
                        markers.Add(Instantiate(markerPrefab, c.transform));
                    }
                }
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator SetToDefault(Controller c) {
        c.GetComponent<MeshRenderer>().material = damagedMaterial;
        yield return new WaitForSeconds(.2f);
        if (!hitEnems.Contains(c) && c != null) c.GetComponent<MeshRenderer>().material = defaultMaterial;
    }



    public override void OnClick() {
        hitEnems = new();
        pc.StartCoroutine(CheckForEnemies());
        source = pc.GetComponent<AudioSource>();
        PlayClip(onClickClip);
    }

    public override void OnRelease() {

        source.Stop();
        
        markers?.ForEach((i) => Destroy(i));

        int enemies = hitEnems.Count;
        if (enemies > 0 && onDamageClip!=null) PlayClip(onDamageClip);
        // Peaks at 10 enemies for double damage
        float damageMultiplier = Mathf.Clamp(1 + (Mathf.Pow(enemies, 2) / 100), 1, 2);

        foreach (Controller c in hitEnems) {
            pc.StartCoroutine(SetToDefault(c));
            c.TakeDamage(damage * damageMultiplier, element);

        }

        pc.StartCoroutine(AttackCooldown());
    }


}