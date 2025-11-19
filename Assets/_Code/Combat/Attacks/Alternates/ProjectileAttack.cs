using UnityEngine;
using Combat.Attacks;

[CreateAssetMenu(fileName = "New ProjectileAttack", menuName = "Attacks/Create/Projectile Attack")]
public class ProjectileAttack : AlternateAttack {
	public string projectileName = "IceSpear";
	const string projectileRoot = "Prefabs/Combat/Projectiles/";
	string projectileAddress => projectileRoot + projectileName;
	GameObject Projectile;

	public override void OnClick() {
		if (Projectile == null) Projectile = Resources.Load<GameObject>(projectileAddress);
		//FIXME: This will only allow the player to use the attack (Not like the enemies have support anyways)
		GameObject obj = Instantiate((Projectile), pc.transform.position, Quaternion.identity);
	}
}
