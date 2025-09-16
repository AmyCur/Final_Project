using Globals;
using MathsAndSome;
using System.Collections;
using UnityEngine;
using Vortex;

[CreateAssetMenu(fileName = "Wind Attack", menuName = "Attacks/Create/Wind", order = 2)]
public class WindAttack : Attack {
    GameObject upVortex;
    GameObject downVortex;
    GameObject cv;

    [Header("Wind")]
    public float upDamage = 5f;
    public float downDamage = 45f;


    void CreateDamageSphere(float dmg) {
        Collider[] ec = Physics.OverlapSphere(pc.transform.position, upVortex.GetComponent<VortexController>().radius);

        foreach (Collider col in ec) {
            EntityController entityController = col.GetComponent<EntityController>();
            if (entityController != null) {
                entityController.TakeDamage(dmg, element);
            }
        }
    }

    IEnumerator Slam() {
        pc.canSlam = false;
        if (cv!=null) Destroy(cv);
        pc.SV.Add(new(new(0, -60, 0), pc));
        yield return new WaitUntil(() => pc.Grounded());
        CreateDamageSphere(downDamage);
        cv = Vortex.Create.CreateVortex(downVortex, pc.transform.position);
        pc.canSlam = true;
    }

    IEnumerator CreateVortex() {

        pc.canSlam = false;

        cv = Vortex.Create.CreateVortex(upVortex, pc.transform.position);

        CreateDamageSphere(upDamage);

        pc.SV.Add(new(new(0, 20, 0), pc));
        yield return new WaitForSeconds(.4f);
        pc.canSlam = true;
    }

    public override void OnALtClick() {
        if (canAltAttack) {
            base.OnALtClick();
            upVortex =Resources.Load<GameObject>("Prefabs/Combat/Projectiles/UpPlayerVortex");
            downVortex=Resources.Load<GameObject>("Prefabs/Combat/Projectiles/DownPlayerVortex");
            if (pc.Grounded()) pc.StartCoroutine(CreateVortex());
            else pc.StartCoroutine(Slam());
        }
    }
}
