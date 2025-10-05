using UnityEngine;

public class BreakableWall : EntityController {
    public override void Start() {
        base.Start();
    }
    public override void Die() {
        base.Die();
        Destroy(gameObject);
    }


}
