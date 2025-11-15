using UnityEngine;

[CreateAssetMenu(fileName = "New double jump", menuName = "Attacks/Create/Double Jump")]
public class Doublejump : AlternateAttack {
	public float jumpForce = 30f;

	public override void OnClick() {
		pc.rb.AddForce(0, jumpForce * Time.deltaTime * Player.Consts.Multipliers.JUMP_MULTIPLIER, 0);
		pc.StartCoroutine(AttackCooldown());
	}
}