using GunUtils;
using System;
using UnityEngine;

namespace GunUtils {
	public enum AttackType {
		single,
		automatic,
		burst
	}

	public enum AttackMode {
		raycast,
		projectile
	}
}

[Serializable]
[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Create Gun", order = 0)]
public class Gun : ScriptableObject {

	public float damage;
	public float shotCD;

	public int bulletsPerShot;

	public AttackType attackType;
	public AttackMode attackMode;

}
