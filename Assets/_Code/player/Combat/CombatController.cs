using Magical;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour {

	[Header("Controls")]
	[Header("Shooting")]

	[Header("Weapons")]
	[Tooltip("Current Weapon")][SerializeField] Attack ca;
	[SerializeField] List<Attack> attacks;
	PlayerController pc;

	void Update() {
		if (ca.canAttack) {
			if (magic.key.down(keys.attack)) {
				ca.OnClick();
			}
			if (magic.key.up(keys.attack)) {
				ca.OnRelease();
			}
		}

		// 49 -> 57
		SwitchWeapon();
	}

	void Start() {
		pc = GetComponent<PlayerController>();
	}

	void SwitchWeapon() {
		int pressedKey = (int) magic.key.PressedKey();
		if (pressedKey >= 49 && pressedKey <= 57) {
			pressedKey -= 49;
			if (attacks.Count >= pressedKey + 1) {
				ca = attacks[pressedKey];
			}
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
	// 				EnemyController enemy = hit.collider.GetComponent<EnemyController>();
	// 				if (enemy == null) {
	// 					Debug.LogError($"EnemyController of {hit.collider.name} is not set");
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