using UnityEngine;
using static EntityLib.Entity;
using static Cur.Settings.Combat;
using MathsAndSome;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Combat {
	public class RangedEnemy : ENM_Controller {

		public enum MovementChoice {
			to_player,
			walk_random,
			retreat
		}


		readonly MovementChoice[] MovementChoices = { MovementChoice.to_player, MovementChoice.walk_random, MovementChoice.retreat };
		MovementChoice choice = MovementChoice.to_player;
		System.Random rand = new System.Random();

		[Header("Hunting Choices")]
		[SerializeField] protected MovementChoice rc = MovementChoice.to_player;
		[SerializeField] protected float movementChoiceTime = 5f;
		[SerializeField] protected Vector2[] Offset = new Vector2[2];
		[SerializeField] protected float retreatDistance;
		protected bool canChangeHuntChoice = true;

		protected GameObject bullet;
		[SerializeField] protected string bulletPath = "Prefabs/Combat/Projectiles/Bullet";

		protected GameObject pipe;
		[SerializeField] protected string pipePath = "Prefabs/Combat/Projectiles/Pipe";

		readonly Dictionary<MovementChoice, float> MovementChoiceMultiplier = new() {
		{MovementChoice.to_player, 1f},
		{MovementChoice.retreat, .8f},
		{MovementChoice.walk_random, .35f}
	};

		protected IEnumerator MovementChoiceCD() {
			canChangeHuntChoice = false;
			yield return new WaitForSeconds(movementChoiceTime * MovementChoiceMultiplier[choice]);
			canChangeHuntChoice = true;
		}

		Vector3 RandomOffset() {
			return new Vector3(
				Mathf.Lerp(Offset[0].x, Offset[1].x, (float) rand.NextDouble()),
				0,
				Mathf.Lerp(Offset[0].y, Offset[1].y, (float) rand.NextDouble())
			);
		}

		Vector3 BackwardsVector() => -(direction * Mathf.Clamp((float) rand.NextDouble(), 0.5f, 1f));


		public override void Seek() { }
		public override void Hunt() {
			if (canChangeHuntChoice) {
				// 0 -> 9
				int randint = new System.Random().Next(10);

				if (randint <= 7) choice = MovementChoices[0];

				else {
					randint -= 7;
					choice = MovementChoices[randint];

				}
				StartCoroutine(MovementChoiceCD());
				Debug.Log(choice);
			}

			Vector3 destination = choice switch {
				MovementChoice.to_player => playerPosition + BackwardsVector(),
				MovementChoice.walk_random => RandomOffset(),
				MovementChoice.retreat => playerPosition + (BackwardsVector() * retreatDistance),
				_ => transform.position
			};

			agent.destination = destination;
		}

		IEnumerator Shoot() {
			for (int i = 0; i < (attackData as RangedData).bulletsPerShot; i++) {
				GameObject bulletObj = Instantiate(bullet, transform.position, Quaternion.identity);
				if (bulletObj.TryGetComponent<BulletController>(out BulletController bc)) {
					bc.damage = attackData.damage;
					bc.parent = GetComponent<CapsuleCollider>();
				}
				bulletObj.transform.LookAt(playerPosition);
				yield return new WaitForSeconds((attackData as RangedData).timeBetweenBurstShot);
			}
		}

		public override void Attack() {
			// If the player is in line of sight
			if (canAttack && Physics.Raycast(pos, direction * attackData.attackRange)) {
				agent.destination = transform.position;
				switch ((attackData as RangedData).projectile) {
					case Projectile.bullet:
						StartCoroutine(Shoot());
						break;
				}

				StartCoroutine(CooldownAttack());
			}

		}

		public override bool shouldHunt() {
			foreach (RaycastHit hit in Physics.RaycastAll(pos, direction, maxHuntRange)) {
				// Debug.Log(hit.collider.name);
				if (hit.collider.isEntity(typeof(Player.PL_Controller))) return hit.distance < maxHuntRange && canHunt && !attackOnCD;
			}

			return false;
		}

		public override bool shouldSeek() {
			foreach (RaycastHit hit in Physics.RaycastAll(pos, direction, maxSeekRange)) {
				if (hit.isEntity(typeof(Player.PL_Controller))) return hit.distance < maxSeekRange && canSeek;
			}

			return false;
		}

		public override bool shouldAttack() {
			// To attack it should have line of sight
			if (Physics.Raycast(pos, direction, out RaycastHit hit, maxSeekRange)) return hit.isEntity() && hit.distance < maxAttackRange && canAttack;
			return false;
		}

		public override void Update() {
			base.Update();
		}

		public override void Start() {
			base.Start();
			rand = new System.Random();

			bullet = Resources.Load<GameObject>(bulletPath);
			pipe = Resources.Load<GameObject>(pipePath);
		}
	}
}