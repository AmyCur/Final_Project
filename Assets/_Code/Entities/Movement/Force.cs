namespace Entities.Movement;

[Serializable]
public class DashForce : Force {
	[Header("Dash Exclusive")]
	public float hardCDTime = 0.1f;
	[Range(0, 100)] public float staminaPer = 30f;

	[Header("Gravity")]
	public float gravityDashForce = 50f;

	[Header("No Gravity")]
	public float noGravDashTime = 0.3f;
}

[Serializable]
public class Force {
	[Header("Bools")]
	public bool can = true;
	bool[] goneBack = new bool[2];

	[Header("Force")]
	public float force = 45f;
	[Range(500, 2000)] public int decayIncrements = 1000;
	public float decaySpeed = 10f;

	[Header("Directions")]

	public Vector3 direction;
	[Range(0.01f, 0.075f)] public float directionChangeSpeed = 0.03f;

	[Header("State")]
	public MovementState state = MovementState.none;

	public void ChangeDirection(Vector3 moveDirection) {
		Vector3 moveD = new(Math.Sign(moveDirection.x), 0, Math.Sign(moveDirection.z));
		Vector3 forceD = new(Math.Sign(this.direction.x), 0, Math.Sign(this.direction.z));

		// Check if the player is moving in the opposite direction of the dash and if they are, change the dash direction to suit them

		if (-moveD.x == forceD.x && moveD.x != 0 && forceD.x != 0) {
			if (this.direction.x <= 0) this.direction = new(this.direction.x + this.directionChangeSpeed, this.direction.y, this.direction.z);
			else if (this.direction.x >= 0) this.direction = new(this.direction.x - this.directionChangeSpeed, this.direction.y, this.direction.z);
			goneBack[0] = true;
		}

		if (-moveD.z == forceD.z && moveD.z != 0 && forceD.z != 0) {
			if (this.direction.z <= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z + this.directionChangeSpeed);
			else if (this.direction.z >= 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z - this.directionChangeSpeed);
			goneBack[1] = true;
		}
	}
}
