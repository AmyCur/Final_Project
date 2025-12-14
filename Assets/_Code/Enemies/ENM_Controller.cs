using UnityEngine;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using Elements;
using Combat;

public abstract class ENM_Controller : RB_Controller {

	public Dictionary<string, object> thoughts;


	public enum EnemyState {
		seeking,
		hunting,
		attacking
	}

	protected Vector3 pos => transform.position;
	protected Vector3 direction => (playerPosition - pos).normalized;

	[Header("Enemy")]
	[Header("State")]
	public EnemyState eState;
	protected Player.PL_Controller pc;
	protected Vector3 playerPosition => mas.player.Player.transform.position;

	[Header("Seeking")]

	[SerializeField] protected float maxSeekRange = 10f;

	[Header("Hunting")]
	[SerializeField] protected float maxHuntRange = 10f;

	[Header("Attacking")]

	[SerializeField] protected float maxAttackRange = 10f;
	[SerializeField] protected AttackData attackData;


	[Header("Cans")]

	[SerializeField] protected bool canSeek = true;
	[SerializeField] protected bool canHunt = true;
	[SerializeField] protected bool canIdle = true;
	[SerializeField] protected bool canAttack = true;
	public bool displayDebugInfo;

	[Header("Elements")]
	[SerializeField] protected Element attackElement;

	protected NavMeshAgent agent;

	protected bool attackOnCD;


	protected IEnumerator CooldownAttack() {
		attackOnCD = true;
		canAttack = false;
		yield return new WaitForSeconds(
			attackData.attackCD +
			((attackData as RangedData).timeBetweenBurstShot * ((attackData as RangedData).bulletsPerShot - 1))
		);
		canAttack = true;
		attackOnCD = false;
	}

	public abstract bool shouldHunt();
	public abstract bool shouldSeek();
	public abstract bool shouldAttack();

	public abstract void Hunt();
	public abstract void Seek();
	public abstract void Attack();

	public override void UpdateThoughts() {
		thoughts = new() {
			{nameof(health), health},
			{nameof(defence), defence},
			{nameof(shouldHunt), shouldHunt()},
			{nameof(shouldSeek), shouldSeek()},
		};

		for (int i = 0; i < transform.childCount; i++) {
			if (!!transform.GetChild(i) && transform.GetChild(i).CompareTag("Thought")) transform.GetChild(i).gameObject.GetComponent<ThoughtBubble>().SetText(thoughts);
		}
	}

	public override void Update() {
		base.Update();

		//* Should be in this order so the enemy isnt stuck seeking!!!!

		if (shouldAttack()) Attack();
		else if (shouldHunt()) Hunt();
		else if (shouldSeek()) Seek();


	}

	// public override void OnValidate() { base.OnValidate(); CheckRanges();}


	public override void Start() {
		base.Start();
		pc = mas.player.Player;
		if (TryGetComponent<NavMeshAgent>(out NavMeshAgent ag)) agent = ag;
	}



	public override void Die() { base.Die(); Destroy(gameObject); }

	void OnDrawGizmos() {
		if (displayDebugInfo) {
			Gizmos.color=Color.red;
			Gizmos.DrawWireSphere(transform.position, maxSeekRange);
			Gizmos.color=Color.blue;
			Gizmos.DrawWireSphere(transform.position, maxHuntRange);
			Gizmos.color=Color.green;
			Gizmos.DrawWireSphere(transform.position, maxAttackRange);
		}
	}

}