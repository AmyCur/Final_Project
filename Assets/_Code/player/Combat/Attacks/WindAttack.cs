using Globals;
using MathsAndSome;
using System.Collections;
using UnityEngine;
using Vortex;

[CreateAssetMenu(fileName = "Wind Attack", menuName = "Attacks/Create/Wind", order = 2)]
public class WindAttack : Attack {
    [SerializeField]GameObject upVortex;
    [SerializeField]GameObject downVortex;
    GameObject cv;

    void Start() {
        Debug.Log("Staaaaart");
    }

    public override void OnClick() {
        base.OnClick();
    }

    IEnumerator Slam() {
        if (cv!=null) Destroy(cv);
        pc.SV.Add(new(new(0, -60, 0), pc));
        yield return new WaitUntil(() => pc.Grounded());
        cv = Vortex.Create.CreateVortex(downVortex, pc.transform);
        pc.canSlam = true;
    }

    IEnumerator CreateVortex() {


        pc.canSlam = false;

        cv = Vortex.Create.CreateVortex(upVortex, pc.transform);
        pc.SV.Add(new(new(0, 20, 0), pc));
        yield return new WaitForSeconds(.4f);
        pc.StartCoroutine(Slam());
    }

    public override void OnALtClick() {
        if (canAltAttack) {
            base.OnALtClick();
            // upVortex=Resources.Load<GameObject>("/Prefabs/Combat/Projectiles/UpPlayerVortex");
            // downVortex=Resources.Load<GameObject>("/Prefabs/Combat/Projectiles/DownPlayerVortex");
            if (pc.Grounded()) pc.StartCoroutine(CreateVortex());
            else pc.StartCoroutine(Slam());
        }
    }
}
