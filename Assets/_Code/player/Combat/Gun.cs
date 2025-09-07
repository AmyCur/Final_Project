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

	[Header("Attack Stats")]
	public float damage;
	public float shotCD;
	public bool canShoot = true;
	
	[Header("Bullet Stats")]
	public float maxRange;
	public int bulletsPerShot;
	public Vector2[] shotOffset = { Vector2.zero, Vector2.zero };

	[Header("Attack Stats")]
	public AttackType attackType;
	public AttackMode attackMode;

}
