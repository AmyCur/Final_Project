using Input.Magical;
using UnityEngine;


namespace Combat.Attacks;

[CreateAssetMenu(fileName="Primary Attack", menuName="Attacks/Create/Primary")]
public class PrimaryAttack : SingularAttack {
    public override bool keyDown() => magic.key.down(keys.attack);
    public override bool keyStayDown() => magic.key.gk(keys.attack);
    public override bool keyUp() => magic.key.up(keys.attack);
	
	GameObject projectile=null;
	const string projectilePath = "Prefabs/Combat/Projectiles/Primary";

	public override void OnClick(){
		if(projectile==null) projectile = Resources.Load<GameObject>("Prefabs/Combat/Projectiles/Primary");
		GameObject proj = Instantiate(projectile, pc.playerCamera.transform.position, Quaternion.identity);
		proj.transform.parent=GameObject.FindWithTag("Bullets").transform;

		base.OnClick();
	}
}