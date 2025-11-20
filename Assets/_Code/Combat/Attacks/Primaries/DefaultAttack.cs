using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;


[CreateAssetMenu(fileName = "Default Attack", menuName = "Attacks/Primary/Default", order = 0)]
public class DefaultAttack : PrimaryAttack {

	GameObject projectile;

	public override void OnClick() {

		Instantiate(projectile, pc.transform.position + (pc.transform.forward * projectile.transform.localScale.x), Quaternion.identity);

		base.OnClick();
	}

	void Awake() {
		projectile = Resources.Load<GameObject>("Prefabs/Combat/Projectiles/PlayerAttack");
	}

}