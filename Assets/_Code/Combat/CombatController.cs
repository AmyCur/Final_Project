using Globals;
using Magical;
using MathsAndSome;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour {

	[Header("Controls")]
	[Header("Shooting")]

	[Header("Weapons")]
	[Tooltip("Current Weapon")][SerializeField] Attack ca;
	public List<Attack> attacks;
	HUDController hc;
	Player.PL_Controller pc;

	void Update() {
		if(!!ca){
			if (ca.primary.keyDown() && ca.primary.canAttack) ca.primary.OnClick();
			if (ca.primary.keyUp()) ca.primary.OnRelease();

			if (ca.alt.keyDown() && ca.alt.canAttack) ca.alt.OnClick();
			if (ca.alt.keyUp()) ca.alt.OnRelease();
		}
		// 49 -> 57
		SwitchWeapon();
	}

	void Start() {
		pc = GetComponent<Player.PL_Controller>();
		hc = mas.get.HC();
		if (!ca) ca = attacks[0];
		UpdateIcons();
	}

	void UpdateIcons() {
		hc.UpdateIcon(ca.primary.element.type);
	}

	void SwitchWeapon() {
		int pressedKey = (int) magic.key.PressedKey();
		if (pressedKey >= 49 && pressedKey <= 57) {
			pressedKey -= 49;
			if (attacks.Count >= pressedKey + 1) {
				ca = attacks[pressedKey];
				UpdateIcons();
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