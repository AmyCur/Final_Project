using UnityEngine;
using MathsAndSome;
using System.Collections.Generic;

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
	protected Vector3 playerPosition=>mas.player.GetPlayer().transform.position;

	[Header("Seeking")]

	[SerializeField] protected float minSeekRange = 10f;
	[SerializeField] protected float maxSeekRange = 10f;

	[Header("Hunting")]
	[SerializeField] protected float minHuntRange = 10f;
	[SerializeField] protected float maxHuntRange = 10f;

	[Header("Cans")]

	[SerializeField] protected bool canSeek = true;
	[SerializeField] protected bool canHunt = true;
	[SerializeField] protected bool canIdle = true;

	void CheckRanges() => maxHuntRange = Mathf.Clamp(maxHuntRange, minSeekRange, Mathf.Infinity);

	public abstract bool shouldHunt();
	public abstract bool shouldSeek();

	public abstract void Hunt();
	public abstract void Seek();

	public override void UpdateThoughts()
	{
		thoughts = new() {
			{nameof(health), health},
			{nameof(defence), defence},
			{"shouldHunt", shouldHunt()},
			{"shouldSeek", shouldSeek()},
		};

		for (int i = 0; i < transform.childCount; i++)
		{
			if (!!transform.GetChild(i) && transform.GetChild(i).CompareTag("Thought")) transform.GetChild(i).gameObject.GetComponent<ThoughtBubble>().SetText(thoughts);
		}
	}

	public override void Update()
	{
		base.Update();
		if (shouldHunt()) Hunt();
		if (shouldSeek()) Seek();

		
	}

	// public override void OnValidate() { base.OnValidate(); CheckRanges();}


	public override void Start() {
		base.Start();
		CheckRanges();

		
	}



	public override void Die() { base.Die(); Destroy(gameObject); }

}