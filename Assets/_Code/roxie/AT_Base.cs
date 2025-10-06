// using UnityEngine;

// using System;
// using System.Collections;
// using System.Collections.Generic;

// /* 
//     Animations needed:
//         - attackDisplay
//             - idle
//             - attack
//         - crosshair
//             - idle
// */

// [CreateAssetMenu(fileName = "new attack", menuName = "attacks/base")]
// public class AT_base : ScriptableObject {
//     [Header("basic values")]
//     [Range(0f, 25f)] public float range = 25f;
//     [Range(0f, 25f)] public float shootDelay = 1f;
//     public bool canShoot = true;

//     [Header("display")]
//     public string name = "base";

//     public Sprite sprite;

//     public RuntimeAnimatorController AC; 
//     public RuntimeAnimatorController crosshair;

//     [Header("sounds")]
//     public AudioClip SF_fire;

//     [Header("ammo")]
//     public bool useAmmo = true;
//     public int useageAmmo = 1;
//     public int maxAmmo = 6;
//     public int currentAmmo = 6;

//     [Header("damage")]
//     [Range(0f, 25f)] public float damage = 10f;

//     /*
//         This method is called when first loading the attack into the player
//         you can use this to load default values or add animations like i am doing
//     */
//     public virtual void load(PL_Controller character) {
//         // animators
//         // character.AttackDisplay.runtimeAnimatorController = AC;
//         // character.crosshairDisplay.runtimeAnimatorController = crosshair;

//         // data
//         canShoot = true;
//         currentAmmo = maxAmmo;
//     }

//     /*
//         This method is called when the attack is taken off the player
//         use this to un do whatever load does typically
//     */
//     public virtual void unLoad(PL_Controller character) {
//         // character.Reset();
//     }

//     /*
//         Basic attack, you can use this for whatever really
//     */
//     public virtual void attack(PL_Controller character) {
//         if (!canShoot) return; // if the attack cannot be used return
//         if (useAmmo && (currentAmmo - useageAmmo) < 0) return; // if the attack uses ammo and the user has no ammo, return

//         // effects
//         // character.Shot_effect.Play("flash");
//         // character.AttackDisplay.Play("attack");
        
//         // attack itself
//         Collider hit = runHit(character);
    
//         // do something with the attack
//         if (hit) {
//             ENM_Controller enemey = null;

//             // character.AS.clip = SF_fire;
//             // character.AS.Play();

//             if ((enemey = hit.transform.GetComponent<ENM_Controller>()) != null) {
//                 // sound
//                 // enemey.DealDamage((int)damage, character.transform);
//             }
//         }

//         // shoot delay
//         character.StartCoroutine(fireCondition());

//         // lower ammo
//         if (useAmmo) currentAmmo -= useageAmmo;
//     }

//     /*
//         I use this to define the condition i want the player to fire under, so a simple delay will do for the basic attack
//     */
//     public virtual IEnumerator fireCondition() {
//         canShoot = false;
//         yield return new WaitForSeconds(shootDelay);
//         canShoot = true;
//     }

//     /*
//         This is what ill use to get a target, so i can run this multiple times with multiple offsets for something like a shotgun
//     */
//     public Collider runHit(PL_Controller character, float offset = 0) {
//         RaycastHit hit;
//         Vector3 targetDirection = Vector3.forward + new Vector3(offset, 0, 0);

//         if (Physics.Raycast(character.transform.position, character.transform.TransformDirection(targetDirection), out hit, range)) { 
//             Debug.DrawRay(character.transform.position, character.transform.TransformDirection(targetDirection) * hit.distance, Color.red, 2); 
        
//             Debug.Log($"hit target: {hit.collider.gameObject.name}"); // debug

//             return hit.collider;
//         }

//         return null;
//     }

//     /*
//         reload, i have no plans to add a way to reload into the actual game, meaning that is should be called with a special ability of some kind
//     */
//     public void reload() {
//         currentAmmo = maxAmmo;
//     }
// }