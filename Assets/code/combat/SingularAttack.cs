using Combat.Elements;
using Entity;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat.Enemies;
using Combat.Attacks.Primary;
using System.Threading.Tasks;


// Weapon types
// 0 = Shoot
// 1 = Swing


public abstract class SingularAttack : ScriptableObject {

	protected AudioSource source;
	protected Animator shootingAnimation=> GameObject.Find("Weapon").GetComponent<Animator>();
	// public AnimatorController weaponAnimatorController;

	protected Entity.ENT_Controller[] hitEnemies(Vector3 startPos, Vector3 direction, float range) {
		Debug.DrawLine(startPos, startPos + (direction * range), Color.red, 1);
		List<Entity.ENT_Controller> ecs = new();
		Vector3 pos = startPos;
		float dist = range;

		RaycastHit[] hits = Physics.RaycastAll(startPos, direction, range);


		if (hits.Length > 0) {
			foreach (RaycastHit hit in hits) {
				if (hit.isEntity<ENM_Controller>()) {
					Entity.ENT_Controller ec = hit.collider.GetComponent<Entity.ENT_Controller>();
					if (!!ec) {
						ecs.Add(ec);
					}
				}
			}
		}

		if (ecs.Count > 0) {
			return ecs.ToArray();
		}

		return null;
	}

	protected async void SetAnimation(){
		// shootingAnimation ??= GameObject.Find("Weapon").GetComponent<Animator>();
		// Son 
		shootingAnimation.SetBool("shoot",true);
		await Task.Delay(100);
		shootingAnimation.SetBool("shoot",false);
	}


	protected void OnClickShared(){
		SetAnimation();
		//! This is causing crash
		this.OnClick();
	}


	protected Player.Movement.PL_Controller pc => mas.player.Player;

	public enum AttackType {
		single,
		hold
	}


	[Header("Stats")]
	public AttackType attackType;
	public float damage;
	public float attackCD;
	public float range;
	public bool canAttack = true;
	public bool hasHoming;

	[Header("Times")]
	public float pullOutTime;

	[Header("Elements")]
	public Element element;

	[Header("Audio")]
	public AudioClip onClickClip;
	public AudioClip onReleaseClip;
	public AudioClip onDamageClip;

	[Header("Life Steal")]
	public float lifeStealOnHit;

	[Header("Visuals")]

	public Sprite weaponSprite;

	protected void PlayClip(AudioClip clip) { if (!!clip) source.PlayOneShot(clip); }

	public virtual bool keyDown() => false;
	public virtual bool keyUp() => false;
	public virtual bool keyStayDown() => false;

	public virtual void OnClick() { pc.StartCoroutine(AttackCooldown()); }
	public virtual void OnClickHoming(){}
	public virtual void OnRelease() { }

	public virtual IEnumerator AttackCooldown() {
		canAttack = false;
		yield return new WaitForSeconds(attackCD);
		if(this is MeleeAttack atk){
			atk.animator.SetBool("Attacking", false);
			pc.StartCoroutine(atk.SwingCD());
		}
		canAttack = true;
	}

	
}