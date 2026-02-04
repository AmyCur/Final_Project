using UnityEngine;
using MathsAndSome;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using Combat.Elements;

namespace Combat.Enemies{
	public abstract class ENM_Controller : RB_Controller {
		
		public Dictionary<string, object> thoughts;

		public enum EnemyState {
			seeking,
			hunting,
			attacking
		}

		public bool isEnabled;

		protected NavMeshAgent agent;

		protected Vector3 pos => transform.position;
		protected Vector3 direction => (playerPosition - pos).normalized;

		[Header("Enemy")]
		[Header("State")]
		public EnemyState eState;
		protected Player.Movement.PL_Controller pc=>mas.player.Player;
		protected Vector3 playerPosition => mas.player.Player.transform.position;

		[Header("Attacking")]

		public float damage=10f;
		public float attackCD=0.5f;
		public float range=3f;
		[SerializeField] protected bool canAttack=true;

		[Header("Debuffs")]
		public bool staggered;

		public IEnumerator Stagger(float staggerTime){
			staggered = true;
			if(agent!=null) agent.destination=transform.position;
			yield return new WaitForSeconds(staggerTime);
			staggered = false;
		}
		



		// This is the currently executed routine, this should never be null and if it is (i.e. recompile while game is running) it will be rerstarted
		protected Coroutine aiRoutine;
		protected Coroutine combatRoutine;


		protected abstract bool ShouldHunt();
		protected abstract bool ShouldAttack();

		protected abstract void Hunt();
		protected virtual void Attack(){
			StartCoroutine(AttackCooldown());
		}

		protected IEnumerator AttackCooldown(){
			canAttack=false;
			yield return new WaitForSeconds(attackCD);
			canAttack=true;
		}


		protected IEnumerator HuntRoutine(){
			eState = EnemyState.hunting;
			while (true){
				
				yield return 0;
				if(!staggered) Hunt();
				if (ShouldAttack()) SwitchRoutine(); 
			}
		}

		protected IEnumerator AttackRoutine(){
			eState = EnemyState.attacking;
			while (true){
				yield return 0;		
				if(!staggered) Attack();
				if (ShouldHunt()) SwitchRoutine(); 
			}
		}


		protected IEnumerator HandleAIChoices(){
			
			// If the ais doing nothing
			do {
				if (isEnabled){
					if(ShouldAttack()) combatRoutine = StartCoroutine(AttackRoutine());
					else if(ShouldHunt()) combatRoutine = StartCoroutine(HuntRoutine()); 
				}
				
				yield return 0;
			} while (combatRoutine == null);

		}

		protected void SwitchRoutine(){
			StopCoroutine(combatRoutine);
			StartCoroutine(HandleAIChoices());
		}

		public override void Start(){
			base.Start();

			aiRoutine = StartCoroutine(HandleAIChoices());
			agent = GetComponent<NavMeshAgent>();
		}

		
		public override void Die() => Destroy(gameObject);

	}
}
