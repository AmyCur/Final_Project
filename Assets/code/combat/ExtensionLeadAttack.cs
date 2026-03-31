using Audio;
using Entity;
using Magical;
using UnityEngine;

namespace Combat {
	[CreateAssetMenu(fileName="Extension Lead Attack", menuName="Attacks/Primary/Extension Lead")]
	public class ExtensionLeadAttack : SingularAttack{

		[SerializeField] GameObject sparkAttack;

		public override bool keyDown() => magic.key.down(keys.attack);
		public override bool keyStayDown() => magic.key.gk(keys.attack);
		public override bool keyUp() => magic.key.up(keys.attack);

		public override void OnClick() {
			
			GameObject spark = Instantiate(sparkAttack, pc.cameraPos, Quaternion.identity);
			spark.GetComponent<HomingController>().damageSound=onDamageClip;
			spark.GetComponent<HomingController>().element=element;
		
			AudioManager.PlaySoundUntilStop(onClickClip);
			shootingAnimation.SetInteger("weapon_type", 1);
			SetAnimation();
			//! This is causing crash
			base.OnClick();
		}
	}
}
