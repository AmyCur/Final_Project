using Elements;
using EntityLib;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingularAttack : ScriptableObject {

	protected AudioSource source;

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


	protected Player.PL_Controller pc => mas.player.Player;

	public enum AttackType {
		single,
		hold
	}

	public AttackType attackType;
	public float damage;
	public float attackCD;
	public float range;
	public bool canAttack = true;


	public Element element;

	public AudioClip onClickClip;
	public AudioClip onReleaseClip;
	public AudioClip onDamageClip;

	protected void PlayClip(AudioClip clip) { if (!!clip) source.PlayOneShot(clip); }

	public virtual bool keyDown() => false;
	public virtual bool keyUp() => false;
	public virtual bool keyStayDown() => false;

	public virtual void OnClick() { pc.StartCoroutine(AttackCooldown()); }
	public virtual void OnRelease() { }

	public virtual IEnumerator AttackCooldown() {
		canAttack = false;
		yield return new WaitForSeconds(attackCD);
		canAttack = true;
	}
}