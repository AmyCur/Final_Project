/*using MathsAndSome;
using UnityEngine;
using EnemyState;
using System.Collections;
using ColourUtils;
using Globals;
using UnityEngine.UI;
using AttackEnums;
using BossUtils;
using SoundManager;

namespace AttackEnums
{
    public enum AttackType
    {
        ray,
        explosion,
    }

    public enum Attacker
    {
        player,
        swordsman,
        archer,
        menace,
        hookshot,
        projectile,
        silent
    }
}


// Struct that will contain all data about the attack
public class AttackData
{
    public Attacker? attackerName;
    public string victimName;


    public AttackType? aType;               // Nullable because its a bonus 
}

public sealed class RayData : AttackData
{
    public Vector3 startPos;
    public Vector3 direction;

    public float attackRange;

    public RayData(Attacker? atkName, float atkRange, Vector3 sPos, Vector3 tPos)
    {
        attackerName = atkName;
        attackRange = atkRange;
        startPos = sPos;
        direction = tPos;
    }
}

public sealed class ExplosionData : AttackData
{
    public float explosionRadius;
    public Vector3 explosionPosition;

    public ExplosionData(
        Attacker? atkName,
        float explosionRadius,
        Vector3 explosionPosition
    )
    {
        attackerName = atkName;
        this.explosionRadius = explosionRadius;
        this.explosionPosition = explosionPosition;
    }
}


namespace MasterCombatController            // This is a namespace so it can be used by players and enemies
{
    public sealed class MCC
    {

        public static float airMultiplier = 1f;

        public sealed class Ray
        {
            public static RaycastHit? ShootRay(RayData data, LayerMask? mask)
            {

                if (data.attackerName == Attacker.player)
                {
                    var cc = mas.player.GetCombatController();
                    if (!cc.gameObject.GetComponent<PauseController>().paused)
                        sman.PlaySound(cc.bowshoot, out _, cc.GetComponent<AudioSource>());
                }
                if (mask != null)
                {

                    if (Physics.Raycast(data.startPos, data.direction, out RaycastHit hit, data.attackRange, (LayerMask)mask))
                    {
                        return hit;
                    }
                }
                else
                {
                    if (Physics.Raycast(data.startPos, data.direction, out RaycastHit hit, data.attackRange))
                    {
                        return hit;
                    }
                }


                return null;
            }
        }
        public sealed class Explosion
        {
            public static Collider[] CreateExplosion(ExplosionData data)
            {
                return Physics.OverlapSphere(data.explosionPosition, data.explosionRadius);
            }
        }

        public sealed class KB
        {
            static float airMultiplier = 1f;

            public static void TakeKnockback(GameObject obj, Vector3 h, float? upForce)
            {
                //if(hit.GetType() == typeof(RaycastHit)){
                if (h != Vector3.zero && obj.GetComponent<Rigidbody>() is Rigidbody rb)
                {

                    var cc = mas.player.GetCombatController();

                    Debug.Log("Ray");

                    rb.linearVelocity = new Vector3(
                        h.x * cc.currentWeapon.kbForce,
                        upForce ?? 0,
                        h.z * cc.currentWeapon.kbForce
                    );

                    airMultiplier *= 0.66f;
                }   // float y = airMultiplier > 0.5 ? 1 : rb.linearVelocity.y / (cc.currentWeapon.kbForce * airMultiplier) * 2;

            }

        }

        public class Player
        {

            public static readonly Color hurtColour = new(213, 35, 35, 85 / 2);
            public static readonly Color otherColour = new(hurtColour.r, hurtColour.g, hurtColour.b, 0);
            public static Image img = GameObject.FindGameObjectWithTag(glob.imgTag).GetComponent<Image>();

            GameObject mbhHolder = new();

            public static IEnumerator CreateDamageEffect(CombatController cc, float t)
            {
                const float dt = 0.02f;

                img.color = Colours.FTS(Colours.LerpAlphas(hurtColour, otherColour, t));

                yield return new WaitForSeconds(dt);

                if (t < 1)
                {
                    cc.StartCoroutine(CreateDamageEffect(cc, t + 0.08f));
                }
                else
                {
                    img.color = otherColour;
                    cc.er = null;
                }
            }

            public static void HandleDamage(CombatController cc, int damage)
            {
                cc.health -= damage;

                if (damage > 0)
                {
                    try
                    {
                        cc.StopCoroutine(cc.er);
                    }
                    catch { }


                    if (cc.er == null)
                    {
                        cc.er = cc.StartCoroutine(CreateDamageEffect(cc, 0));
                    }
                }


                if (cc.health <= 0)
                {
                    // So displays dont show it as negative
                    cc.health = 0;
                    Die(cc, null);
                }

                else if (cc.health > cc.maxHealth && !cc.god)
                {

                    cc.health = cc.maxHealth;
                }
            }
        }

        // dr for damage reciever
        public static void TakeDamage(object obj, int damage, Vector3 hit, float? upForce, RayData data)
        {
            if (obj is BaseEnemy dr)
            {

                if (dr.canTakeDamage && dr.s != EState.dead)
                {
                    if (dr is MalachaiController mc)
                    {
                        mc.health -= Mathf.FloorToInt(damage * Barrier.CalculateBarrierStrength(mc.barriers.ToArray()) / 100);
                    }
                    else
                    {
                        dr.health -= damage;
                    }
                    // dr.GetComponent<AudioSource>().PlayOneShot(dr.audios[0], 1);
                    if (data.attackerName != null)
                    {
                        if (data.attackerName == Attacker.player)
                        {
                            var cc = mas.player.GetCombatController();

                            if (dr.health > 0)
                            {
                                cc.GetComponent<AudioSource>().PlayOneShot(cc.bowHit);

                            }

                        }
                    }


                    if (hit != Vector3.zero)
                    {
                        if (!((dr is MalachaiController || dr is DisabledEnemy) && dr != null))
                        {
                            KB.TakeKnockback(dr.gameObject, hit, upForce);
                        }
                    }

                    else
                    {
                        Debug.Log("hit==null");
                    }

                    dr.StartCoroutine(dr.ChangeMaterial());


                    if (dr.health <= 0)
                    {
                        Die(obj, data);
                    }

                    dr.StartCoroutine(dr.IFrames());

                }
            }

            else if (obj is CombatController cc)
            {
                Player.HandleDamage(cc, damage);
            }

            else
            {
                Debug.Log($"{obj.GetType()} is not a valid type for function \"Take Damage\" ");
            }

        }


        public static void Die(object obj, RayData killer)
        {
            if (obj is CombatController cc)
            {
                var pc = mas.player.GetPlayer();
                pc.s = PlayerStates.state.dead;
                cc.Die();
            }

            else if (obj is BaseEnemy be)
            {
                if (be.s != EState.dead)
                {
                    if (be.healthToGive != 0)
                    {
                        {
                            MCC.TakeDamage(mas.player.GetCombatController(), -be.healthToGive, Vector3.zero, null, null);
                        }
                    }

                    if (killer.attackerName != AttackEnums.Attacker.silent)
                    {
                        be.GetComponent<AudioSource>().PlayOneShot(mas.player.GetCombatController().bowKill, 1);
                    }
                    //FIX: Enemies are not actually destroyed (Automatically at least)
                    GameObject.Destroy(be.GetComponent<CapsuleCollider>());
                    GameObject.Destroy(be.GetComponent<MeshRenderer>());
                    GameObject.Destroy(be.GetComponent<MeshFilter>());


                }
                be.s = EState.dead;
            }
        }

    }
}

public class MBH : MonoBehaviour { }
*/