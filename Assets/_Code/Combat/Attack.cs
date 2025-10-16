using Elements;
using Magical;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attacks/Create", order = 0)]
public class Attack : ScriptableObject {

    // [Header("Main Attack")]
    // public float damage;
    // public float attackCD;
    // public float range;
    // public bool canAttack = true;
    // protected bool keyDown;
    // public Element element;

    // [Header("Alt Attack")]
    // public float altCD;
    // public float altAttackCD = 5f;
    // public bool canAltAttack = true;
    // protected bool altKeyDown;
    // RadialUpdater ru;

    public PrimaryAttack primary;
    public AlternateAttack alt;

    [HideInInspector] public Player.PL_Controller pc;




    void GetPlayer() {
        if (!pc) {
            pc = mas.player.GetPlayer();
        }
    }


    /*
    protected IEnumerator AltAttackCooldown() {
        if (ru == null)
        {
            try
            {
                ru = GameObject.FindGameObjectWithTag("RadialBar").GetComponent<RadialUpdater>();
            }
            catch { }
        }
        canAltAttack = false;
        const int chunks = 30;
        float altCD = altAttackCD / (float)chunks;
        for (int i = 0; i < chunks+1; i++) {
            ru.UpdateProgress(Mathf.Ceil(i * 100f/chunks));
            yield return new WaitForSeconds(altCD);
        }
        canAltAttack= true;
    }
*/
}