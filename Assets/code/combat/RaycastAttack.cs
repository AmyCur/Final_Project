using Audio;
using Combat.Enemies;
using Entity;
using Magical;
using MathsAndSome;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName="Raycast Attack", menuName="Attacks/Primary/Raycast")]
public sealed class RaycastAttack : SingularAttack {
    public override bool keyDown() => magic.key.down(keys.attack);
    public override bool keyStayDown() => magic.key.gk(keys.attack);
    public override bool keyUp() => magic.key.up(keys.attack);

	[SerializeField] List<AudioClip> attackSounds = new();

	async void PlaySound(GameObject source){
		System.Random r = new System.Random();
		source.GetComponent<AudioSource>().clip = attackSounds[r.Next(0, attackSounds.Count)];
		source.GetComponent<AudioSource>().Play();
		await Task.Delay((int)(source.GetComponent<AudioSource>().clip.length*1000));
		Destroy(source);
	}



	public override void OnClick(){
		Debug.Log("Shoot");

		GameObject player = Instantiate(Resources.Load<GameObject>("Prefabs/Sound/MusicPlayer"));
		PlaySound(player);

		// AudioManager.Play(attackSounds[r.Next(0, attackSounds.Count)]);
		// s.player.GetComponent<AudioSource>().pitch=r.Next(100,130);

		if(Physics.Raycast(pc.playerCamera.transform.position ,pc.playerCamera.transform.forward, out RaycastHit hit, range)){
			
			if(hit.isEntity<ENM_Controller>()){
				hit.collider.GetComponent<ENM_Controller>().TakeDamage(damage, new(Combat.Elements.ElementType.None));
				AudioManager.PlaySoundUntilStop(hit.collider.GetComponent<ENM_Controller>().hurtSound);
			}	
		}	

		SetAnimation();
			//! This is causing crash
			base.OnClick();
	}

    public override void OnClickHoming()
    {
		
		SetAnimation();
			//! This is causing crash
			base.OnClick();
    }
}