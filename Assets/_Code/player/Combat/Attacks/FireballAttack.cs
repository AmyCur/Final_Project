using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Attack", menuName = "Attacks/Create/Fireball", order = 0)]
public sealed class FireballAttack : Attack {

    [Header("Fireball")]
    [SerializeField] GameObject fireball;
    // [SerializeField] List<GameObject> fireballs;

    public override void OnClick() {
        base.OnClick();
        if (canAttack) {
            if (fireball == null) {
                Debug.Log("Null");
            }
            GameObject fb = Instantiate(fireball,
            pc.playerCamera.transform.position,
            Quaternion.identity);
            fb.GetComponent<FireballController>().direction = pc.playerCamera.transform.forward;
            pc.StartCoroutine(AttackCooldown());
        }
    }

    void Update() {

    }


}
