using Magical;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour {


	bool shouldShoot => canShoot && magic.key.down(keys.attack);

	[Header("Controls")]
	[Header("Shooting")]


	[SerializeField] bool canShoot = true;

	[Header("Weapons")]
	[Tooltip("Current Weapon")][SerializeField] Gun cw;
	[SerializeField] List<Gun> guns;

	void Update() {
		if (shouldShoot) {
			DictateShotType();
		}

		// 49 -> 57
		SwitcWeapon();
	}

	void SwitcWeapon() {
		int pressedKey = (int) magic.key.PressedKey();
		if (pressedKey >= 49 && pressedKey <= 57) {
			pressedKey -= 49;
			if (guns.Count >= pressedKey + 1) {
				cw = guns[pressedKey];
			}
		}
	}

	void DictateShotType() {
		switch (cw.attackMode) {
			case GunUtils.AttackMode.raycast:
				StartCoroutine(RaycastShoot(cw));
				break;
			case GunUtils.AttackMode.projectile:
				StartCoroutine(ProjectileShoot(cw));
				break;
		}
	}

	IEnumerator RaycastShoot(Gun currentWeapon) {

		int bps = currentWeapon.bulletsPerShot;

		if (bps <= 0) {
			bps = 1;
		}

		// Shoot the bps amount of shots
		for (int i = 0; i < bps; i++) {

		}




		yield return null;
	}

	void rotate_vector_by_quaternion(Vector3 v, Quaternion q, Vector3 vprime) {
		// Extract the vector part of the quaternion
		Vector3 u = new(q.x, q.y, q.z);

		// Extract the scalar part of the quaternion
		float s = q.w;

		// Do the math
		vprime = 2.0f * dot(u, v) * u
				+ (s * s - dot(u, u)) * v
				+ 2.0f * s * cross(u, v);
	}

	IEnumerator ProjectileShoot(Gun currentWeapon) {
		yield return null;
	}

	IEnumerator Shoot(Gun currentWeapon) {

		yield return new WaitForSeconds(currentWeapon.shotCD);
	}
}