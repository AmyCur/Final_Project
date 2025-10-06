using Globals;
using MathsAndSome;
using System.Collections;
using UnityEngine;
using Vortex;

// [CreateAssetMenu(fileName = "Wind Attack", menuName = "Attacks/Create/Wind", order = 2)]
public class WindAttack : AlternateAttack {
    GameObject upVortex;
    GameObject downVortex;
    GameObject cv;

    float apex;
    float landingY;

    [Header("Wind")]
    public float upDamage = 5f;
    public float downDamage = 45f;


    void CreateDamageSphere(float dmg) {
        Collider[] ec = Physics.OverlapSphere(pc.transform.position, upVortex.GetComponent<VortexController>().radius);

        foreach (Collider col in ec) {
            EntityController entityController = col.GetComponent<EntityController>();
            if (!!entityController) {
                entityController.TakeDamage(dmg, element);
            }
        }
    }

    IEnumerator Slam() {
        pc.canSlam = false;
        apex = pc.transform.position.y;
        if (!!cv) Destroy(cv);
        pc.SV.Add(new(new(0, -60, 0), pc));
        yield return new WaitUntil(() => pc.Grounded());
        landingY = pc.transform.position.y;
        Debug.Log($"YYYY: {apex - landingY}");
        CreateDamageSphere(downDamage*Mathf.Clamp((apex-landingY)/30, 1, 4f));
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

    public override void OnClick() {
        if (canAttack) {
            base.OnClick();
            upVortex =Resources.Load<GameObject>("Prefabs/Combat/Projectiles/UpPlayerVortex");
            downVortex=Resources.Load<GameObject>("Prefabs/Combat/Projectiles/DownPlayerVortex");
            if (pc.Grounded()) pc.StartCoroutine(CreateVortex());
            else pc.StartCoroutine(Slam());
        }
    }
}
