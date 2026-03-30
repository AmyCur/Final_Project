using Audio;
using UnityEngine;

namespace Combat {
	[CreateAssetMenu(fileName="Extension Lead Attack", menuName="Attacks/Primary/Extension Lead")]
	public class ExtensionLeadAttack : SingularAttack{

		[SerializeField] GameObject sparkAttack;

		public override void OnClick() {
			GameObject spark = Instantiate(sparkAttack, pc.cameraPos, Quaternion.identity);
			spark.GetComponent<HomingController>().damageSound=onDamageClip;
			AudioManager.PlaySoundUntilStop(onClickClip);
			
			SetAnimation();
			//! This is causing crash
			base.OnClick();
		}
	}
}
