using UnityEngine;
using MathsAndSome;
using Combat.Attacks.Projectiles;

namespace Combat.Attacks{
	[CreateAssetMenu(fileName = "New ProjectileAttack", menuName = "Attacks/Create/Projectile Attack")]
	public class ProjectileAttack : AlternateAttack {
		public string projectileName = "IceSpear";
		const string projectileRoot = "Prefabs/Combat/Projectiles/";
		string projectileAddress => projectileRoot + projectileName;
		GameObject Projectile;

		public override void OnClick() {
			if (Projectile == null) Projectile = Resources.Load<GameObject>(projectileAddress);
			if (canAttack){
				//FIXME: This will only allow the player to use the attack (Not like the enemies have support anyways)
				GameObject obj = Instantiate(Projectile, pc.transform.position, Quaternion.identity);
				obj.transform.rotation =  Quaternion.LookRotation(Vector3.RotateTowards(obj.transform.forward, mas.player.Player.playerCamera.transform.forward, 1000f*Time.deltaTime, 0f));

				obj.GetComponent<BulletController>().Init(pc.playerCamera.transform.forward,damage);
				// obj.GetComponent<BulletController>().damage=damage;
				// obj.GetComponent<BulletController>().parent=pc.GetComponent<CapsuleCollider>();
			}

			base.OnClick();		
		}
	}
}
