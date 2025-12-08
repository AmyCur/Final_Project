using Globals;
using Magical;
using MathsAndSome;
using System.Collections.Generic;
using UnityEngine;
using Combat.Attacks;

namespace Combat {

	[System.Serializable]
	public class SingleAttack {
		public PrimaryAttack primary;
		public AlternateAttack assist;
		public AlternateAttack ability;
	}

	public class CombatController : MonoBehaviour {


		[Header("Controls")]
		[Header("Shooting")]

		[Header("Weapons")]
		[Tooltip("Current Weapon")] public SingleAttack ca = null;
		[HideInInspector] public int caIndex;
		public List<SingleAttack> attacks;
		Cur.UI.HUDController hc;
		Player.PL_Controller pc;
		Coroutine AltCDBarRoutine;
		public SingleAttack spawner;

		void Update() {
			if (ca != null) {
				if (magic.key.down(keys.attack) && ca.primary.canAttack) ca.primary.OnClick();
				if (magic.key.up(keys.attack)) ca.primary.OnRelease();

				if (magic.key.up(keys.assist) && ca.assist.canAttack) ca.assist.OnClick();
				if (magic.key.down(keys.assist)) ca.assist.OnRelease();

				if (magic.key.up(keys.ability) && ca.ability.canAttack) ca.ability.OnClick();
				if (magic.key.down(keys.ability)) ca.ability.OnRelease();
			}
			// 49 -> 57
			SwitchWeapon();

		}

		void Start() {
			hc = mas.get.HC();
			(spawner.primary as SpawnerController).Start();


			if (ca.primary == null) ca = attacks[0];
			caIndex = 0;
			hc.UpdateWeaponBackgrounds();
			hc.UpdateWeaponScale();
			hc.assistBar.UpdateAbilityIcon(ca.assist.icon);
			hc.abilityBar.UpdateAbilityIcon(ca.ability.icon);


			Invoke("LateStart", 0.1f);
		}

		void LateStart() {
			foreach (SingleAttack atk in attacks) {
				if (atk.primary != null) atk.primary.canAttack = true;
				if (atk.assist != null) {
					atk.assist.canAttack = true;
					atk.assist.cooldownProgress = 0;
				}
				if (atk.ability != null) {
					atk.ability.canAttack = true;
					atk.ability.cooldownProgress = 0;
				}

			}
		}

		// void UpdateIcons() {
		// 	hc.UpdateIcon(ca.primary.element.type);
		// }

		void SwitchWeapon() {
			int pressedKey = (int) magic.key.PressedKey();
			if (pressedKey >= 49 && pressedKey <= 57) {
				pressedKey -= 49;
				if(pressedKey==4){
					ca=spawner;
				}
				else if (attacks.Count >= pressedKey + 1) {
					ca = attacks[pressedKey];
					caIndex = pressedKey;
					hc.UpdateWeapons();

					foreach (Cur.UI.CooldownBar bar in hc.cdBars) {
						if (bar.routine != null) StopCoroutine(bar.routine);
						bar.routine = StartCoroutine(bar.UpdateBarColor(ca.primary.element.type, true));
					}

					hc.assistBar.UpdateAbilityIcon(ca.assist.icon);
					hc.abilityBar.UpdateAbilityIcon(ca.ability.icon);

					// if (AltCDBarRoutine != null) StopCoroutine(AltCDBarRoutine);
					// AltCDBarRoutine = StartCoroutine(hc.UpdateAltCDBarColor(ca.primary.element.type, true));
				}
			}
		}

		void OnValidate() {
			if (hc != null) {
				hc.UpdateAll();
				hc.UpdateWeaponIcons();
			}
		}


		// void DictateShotType() {
		// 	switch (cw.attackMode) {
		// 		case GunUtils.AttackMode.raycast:
		// 			StartCoroutine(RaycastShoot(cw));
		// 			break;
		// 		case GunUtils.AttackMode.projectile:
		// 			StartCoroutine(ProjectileShoot(cw));
		// 			break;
		// 	}
		// }

		// IEnumerator RaycastShoot(Gun currentWeapon) {

		// 	Debug.Log($"Raycast shot {currentWeapon.name}");
		// 	currentWeapon.canShoot = false;

		// 	int bps = currentWeapon.bulletsPerShot;

		// 	if (bps <= 0) {
		// 		bps = 1;
		// 	}

		// 	// Shoot the bps amount of shots
		// 	for (int i = 0; i < bps; i++) {

		// 		Vector3 startPosition = pc.playerCamera.transform.position;
		// 		//TODO: Implement random offset
		// 		Vector3 direction = pc.playerCamera.transform.forward;
		// 		float range = currentWeapon.maxRange;
		// 		Debug.DrawLine(startPosition, startPosition + (direction * range), Color.green, 2);

		// 		if (Physics.Raycast(startPosition, direction, out RaycastHit hit, range)) {
		// 			if (hit.collider.tag == "enemy") {
		// 				ENM_Controller enemy = hit.collider.GetComponent<ENM_Controller>();
		// 				if (enemy == null) {
		// 					Debug.LogError($"ENM_Controller of {hit.collider.name} is not set");
		// 				}
		// 				else {
		// 					enemy.TakeDamage(currentWeapon.damage / bps);
		// 				}
		// 			}
		// 		}
		// 	}

		// 	yield return new WaitForSeconds(cw.shotCD);
		// 	currentWeapon.canShoot = true;

		// }

		// IEnumerator ProjectileShoot(Gun currentWeapon) {
		// 	yield return null;
		// }

		// IEnumerator Shoot(Gun currentWeapon) {

		// 	yield return new WaitForSeconds(currentWeapon.shotCD);
		// }

		// void OnDrawGizmos() {

		// }
	}
}