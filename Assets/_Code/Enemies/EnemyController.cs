using UnityEngine;

public class EnemyController : EntityController {

	public override void Die() { base.Die();  Destroy(gameObject); }

}