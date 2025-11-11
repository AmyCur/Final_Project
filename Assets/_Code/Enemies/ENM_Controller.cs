using UnityEngine;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using Elements;

public abstract class ENM_Controller : RB_Controller
{

	public enum EnemyState
	{
		seeking,
		hunting,
		attacking
	}

	[Header("Enemy")]
	[Header("State")]
	public EnemyState eState;
	protected Player.PL_Controller pc; 
	protected Vector3 playerPosition=>mas.player.GetPlayer().transform.position;

	[Header("Seeking")]

	[SerializeField] protected float minSeekRange = 10f;
	[SerializeField] protected float maxSeekRange = 10f;

	[Header("Hunting")]
	[SerializeField] protected float minHuntRange = 10f;
	[SerializeField] protected float maxHuntRange = 10f;

	[Header("Attacking")]

	[SerializeField] protected float minAttackRange = 10f;
	[SerializeField] protected float maxAttackRange = 10f;
	[Space(10)]
	[SerializeField] protected float attackRange = 10f;
	[SerializeField] protected float attackCD = 1f;
	[SerializeField] protected float damage=10f;
	

	[Header("Cans")]

	[SerializeField] protected bool canSeek = true;
	[SerializeField] protected bool canHunt = true;
	[SerializeField] protected bool canIdle = true;
	[SerializeField] protected bool canAttack = true;

	[Header("Elements")]
	[SerializeField] protected Element attackElement;

	protected NavMeshAgent agent;

    protected bool attackOnCD;

	void CheckRanges() => maxHuntRange = Mathf.Clamp(maxHuntRange, minSeekRange, Mathf.Infinity);

	protected IEnumerator CooldownAttack()
	{
		attackOnCD = true;
		canAttack = false;
		yield return new WaitForSeconds(attackCD);
		canAttack = true;
		attackOnCD = false;
    }

	public abstract bool shouldHunt();
	public abstract bool shouldSeek();
	public abstract bool shouldAttack();

	public abstract void Hunt();
	public abstract void Seek();
	public abstract void Attack();

	public override void UpdateThoughts()
	{
		thoughts = new() {
			{nameof(health), health},
			{nameof(defence), defence},
			{nameof(shouldHunt), shouldHunt()},
			{nameof(shouldSeek), shouldSeek()},
		};

		for (int i = 0; i < transform.childCount; i++)
		{
			if (!!transform.GetChild(i) && transform.GetChild(i).CompareTag("Thought")) transform.GetChild(i).gameObject.GetComponent<ThoughtBubble>().SetText(thoughts);
		}
	}

	public override void Update()
	{
		base.Update();

		//* Should be in this order so the enemy isnt stuck seeking!!!!

		if (shouldAttack()) Attack();
		else if (shouldHunt()) Hunt();
		else if (shouldSeek()) Seek();

		
	}

	// public override void OnValidate() { base.OnValidate(); CheckRanges();}


	public override void Start() {
		base.Start();
		CheckRanges();
		pc = mas.player.GetPlayer();
		if(TryGetComponent<NavMeshAgent>(out NavMeshAgent ag)) agent = ag;
        

		
	}



	public override void Die() { base.Die(); Destroy(gameObject); }

}