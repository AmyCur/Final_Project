using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Magical;
using PlayerStates;
using MathsAndSome;
using Globals;

namespace PlayerStates {
	public enum state {
		walking,
		sliding,
		slamming,
		wall_run,
		dead
	}

	public enum AdminState {
		standard,
		noclip
	}
}

[RequireComponent(typeof(CapsuleCollider))]     // For Collision
[RequireComponent(typeof(AudioSource))]
public class PlayerController : EntityController {

	#region Variables

	readonly Vector2 checkScale = new(.4f, 0.06f);

	void OnDrawGizmos() {
		if (GameDebug.Player.drawJumpCollider) {
			Vector3 scale = gameObject.transform.localScale;
			Vector3 pos = gameObject.transform.position;

			Gizmos.DrawCube(
				new Vector3(pos.x, pos.y - scale.y - (checkScale.y / 2), pos.z),
				new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
			);
		}
	}

	public bool Grounded() {
		Vector3 scale = gameObject.transform.localScale;
		Vector3 pos = gameObject.transform.position;

		List<Collider> colliders = Physics.OverlapBox(
			new Vector3(pos.x, pos.y - scale.y - (checkScale.y / 2), pos.z),
			new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
		).ToList();

		colliders.Remove(GetComponent<CapsuleCollider>());
		foreach (Collider c in colliders.ToList()) {
			if (c.isTrigger) colliders.Remove(c);
			if (glob.isEntity(c.tag)) colliders.Remove(c);
		}

		if (colliders.Count() > 0)
			return true;
		return false;

	}


	bool shouldJump => canJump && magic.key.down(keys.jump) && Grounded();
	bool shouldDash => canDash && magic.key.down(keys.dash) && s != state.sliding && stamina>=dashStamina;
	bool shouldSlide => canSlide && magic.key.down(keys.slide) && Grounded() && s != state.sliding;
	bool shouldSlam => canSlam && magic.key.down(keys.slam) && !Grounded();
	bool shouldRegenerateStamina => !regeneratingStamina && (s != state.sliding || s != state.slamming);

	[Space(20)]
	[Header("Player")]
	[Header("Controls")]
	[Header("State")]
	public state s = state.walking;
	public AdminState adminState = AdminState.standard;
	[SerializeField] bool adminMode;
	[Header("Jumping")]

	public float jumpForce = 12f;
	public bool canJump = true;

	[Header("Camera")]

	public bool canRotate;
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	[Space(10)]
	[Range(-90, -60)][SerializeField] float minY = -60f;
	[Range(60, 90)][SerializeField] float maxY = 60f;
	float currentXRotation;

	[Header("Movement")]

	public float forwardSpeed = 12f;
	public float sidewaysSpeed = 12f;
	float defaultSideways = 12f;
	float halfSideways = 6f;
	float defaultSpeed;

	float hInp;
	float vInp;

	[Header("Dash")]

	public float dashForce;
	public float dashCD;
	public bool canDash = true;
	[Space(10)]
	[SerializeField] float dashReductionIncrement = 0.08f;
	[SerializeField] float dashReductionIncrementTime = 0.1f;

	Vector3 dashDirection;
	float dashForceMultiplier = 0f;

	[Header("Sliding")]

	public float slideForce = 5;
	public bool canSlide = true;
	[Space(10)]
	[SerializeField] float slideReductionIncrement = 0.1f;
	[SerializeField] float slideReductionIncrementTime = 0.1f;

	Vector3 slideDirection;
	float slideForceMultiplier = 0f;

	[Header("Slamming")]
	public bool canSlam = true;
	public Vector3 slamForce = new(0, -50, 0);

	[Header("Stamina")]

	public float stamina = 90f;
	public float maxStamina = 90f;
	public float minStamina = 0f;
	public float staminaPerTick = 5f;
	public float deltaTick = 0.05f;
	bool regeneratingStamina = false;

	public float dashStamina = 30f;

	HUDController hc;




	

	


	[Header("Objects")]
	[Header("Movement Objects")]

	public Camera playerCamera;
	[Space(10)]
	public GameObject forwardObject;


	#endregion

	#region Core Functions
	

	public override void SetStartDefaults() {
		base.SetStartDefaults();
		hc = mas.get.HC();
		defaultSpeed = forwardSpeed;
		defaultSideways = sidewaysSpeed;
		halfSideways = sidewaysSpeed / 2;
		playerCamera = Camera.main;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public override void FixedUpdate() {
		Vector3 p1 = WASDMovement() + DashForce() + slamForce;
		movementVector = p1 + SlideForce(Mathf.Clamp(
			(Mathf.Abs(p1.x)+Mathf.Abs(p1.y)+Mathf.Abs(p1.z)) / 24f, 1f, 5f)
		);
		if (s == state.sliding) movementVector = new(movementVector.x, 0, movementVector.z);
		if (adminState == AdminState.standard) {
			if (canMove) {
				Move();
			}
		}
		else {
			AdminMove();
		}
	}

	public override void Update() {
		base.Update();

		if (shouldRegenerateStamina) StartCoroutine(RegenerateStamina());

		sidewaysSpeed = Grounded() ? defaultSideways : halfSideways;
		if (Grounded()) {
			slamForce = Vector3.zero;
		}

		if (canRotate) HandleMouse();
		if (shouldSlam) Slam();

		if (adminState == AdminState.standard) {

			if (canDash) {
				// dashReductionIncrement = 0.1f;
			}

			if (shouldJump) {
				Jump();
				if (!canDash) {
					// dashReductionIncrement = 0.01f;
				}
			}

			if (shouldDash) {
				slideForceMultiplier = 0f;
				dashDirection = DashDirection();
				StartCoroutine(Dash());
			}
			if (shouldSlide) {
				slideDirection = SlideDirection();
				StartCoroutine(Slide());
			}
		}
		else {
			if (magic.key.down(keys.slide)) {
				if (forwardSpeed == defaultSpeed * 2) {
					forwardSpeed = defaultSpeed * 4;
				}
				else {
					forwardSpeed = defaultSpeed * 2;
				}
			}

			if (magic.key.down(keys.teleport)) {
				if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity, ~16)) {
					transform.position = hit.point;
				}
			}

			if (magic.key.gk(keys.jump)) {
				rb.AddForce(0, jumpForce * 5, 0);
			}

			if (magic.key.gk(keys.dash)) {
				rb.AddForce(0, -jumpForce * 5, 0);
			}
		}

		if (magic.key.down(keys.noclip) && adminMode) {
			if (adminState == AdminState.standard) {
				forwardSpeed = defaultSpeed * 2;
				adminState = AdminState.noclip;
				GetComponent<CapsuleCollider>().isTrigger = true;
				rb.useGravity = false;
			}
			else {
				forwardSpeed = defaultSpeed;
				adminState = AdminState.standard;
				GetComponent<CapsuleCollider>().isTrigger = false;
				rb.useGravity = true;
			}

			rb.linearVelocity = Vector3.zero;
		}
	}
	#endregion

	#region Admin
	void AdminMove() {
		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		Vector3 right = playerCamera.transform.right;
		Vector3 forward = playerCamera.transform.forward;

		rb.linearVelocity = (forward * vInp + right * hInp).normalized * forwardSpeed;
	}
	#endregion

	#region Other Functions

	#region Movement
	

	Vector3 MoveDirection() {
		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		if (hInp != 0 && vInp != 0) {
			slideForceMultiplier = Mathf.Clamp(slideForceMultiplier - 0.03f, 0, Mathf.Infinity);
		}

		Vector3 forward = forwardObject.transform.forward;
		Vector3 right = forwardObject.transform.right;

		return (forward * vInp + right * hInp).normalized;
	}

	void Jump() {
		// rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
		rb.AddForce(new(0, jumpForce * 10, 0));
	}

	Vector3 WASDMovement() {
		if (s != state.sliding) return new(MoveDirection().x * forwardSpeed, 0, MoveDirection().z * forwardSpeed);

		return Vector3.zero;

	}

	#region Stamina
	IEnumerator RegenerateStamina() {
		regeneratingStamina = true;
		while (stamina < maxStamina) {
			stamina = Mathf.Clamp(stamina + (movementVector==Vector3.zero ? staminaPerTick: staminaPerTick*1.5f),minStamina, maxStamina);
			hc.UpdateStaminaBars();
			yield return new WaitForSeconds(deltaTick);
		}
		regeneratingStamina = false;
	}

	public void ReduceStamina(float reduce) {
		stamina -= reduce;
		hc.UpdateStaminaBars();
	}

	#endregion

	#endregion

	#region Directions

	Vector3 DashDirection(bool increaseForward = true) {
		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		Vector3 forward = forwardObject.transform.forward;
		Vector3 right = forwardObject.transform.right;

		// Debug.Log($"Forward {forward} | Right {right}");

		if (s != state.sliding) {
			if (hInp != 0) {
				return vInp != 0 ? (forward * vInp + right * hInp).normalized : (right * hInp).normalized;
			}

			else {
				return vInp != 0 ? (forward * vInp).normalized : increaseForward ? forward * 1.1f : forward /*Forward is increased because it feels smaller cos ur not moving*/;
			}
		}

		return Vector3.zero;


		// return (hInp != 0 && vInp != 0) ? (forward * vInp + right * hInp).normalized : forward;

	}

	Vector3 SlideDirection() {
		return DashDirection(false);
	}
	#endregion

	#region Dashing
	Vector3 DashForce() {
		Vector3 direction = dashDirection;
		direction *= dashForce * dashForceMultiplier;
		if (s != state.sliding) return direction;
		return Vector3.zero;

	}

	IEnumerator ReduceDashForce() {
		while (dashForceMultiplier > 0) {
			dashForceMultiplier -= Grounded() ? dashReductionIncrement : dashReductionIncrement / 10;
			dashForceMultiplier = Mathf.Clamp(dashForceMultiplier, 0, Mathf.Infinity);
			yield return new WaitForSeconds(dashReductionIncrementTime);
		}

		dashForceMultiplier = 0f;
	}

	IEnumerator Dash() {
		ReduceStamina(dashStamina);
		canDash = false;
		dashForceMultiplier = 1f;

		yield return new WaitForSeconds(0.2f);
		StartCoroutine(ReduceDashForce());

		yield return new WaitForSeconds(dashCD);
		canDash = true;
	}
	#endregion

	#region Sliding
	Vector3 SlideForce(float velocity) {
		Debug.Log($"V: {velocity}");
		Vector3 direction = slideDirection;
		direction *= slideForce * slideForceMultiplier*velocity;
		return direction;
	}

	IEnumerator ReduceSlideForce() {
		yield return new WaitForSeconds(.05f);

		while (slideForceMultiplier > 0 || !Grounded()) {
			slideForceMultiplier -= slideReductionIncrement <= 0 ? 0.1f : slideReductionIncrement;
			slideForceMultiplier = Mathf.Clamp(slideForceMultiplier, 0, Mathf.Infinity);
			yield return new WaitForSeconds(slideReductionIncrementTime);
		}

		slideForceMultiplier = 0f;
	}

	IEnumerator LockPlayerToGround(){
		while (s == state.sliding) {

			if (Physics.Raycast(transform.position, new(0, -1, 0), out RaycastHit hit, 3f)) transform.position = new(transform.position.x, hit.point.y + transform.localScale.y, transform.position.z);
			yield return new WaitForSeconds(0f);
		}
	}


	IEnumerator Slide() {

		s = state.sliding;
		canSlide = false;
		slideForceMultiplier = 1f;
		StartCoroutine(LockPlayerToGround());
		yield return new WaitUntil(() => magic.key.up(keys.slide) || shouldJump);
		if (shouldJump) {
			StartCoroutine(ReduceSlideForce());
		}
		else {
			slideForceMultiplier = 0f;
		}
		canSlide = true;
		s = state.walking;

	}
	#endregion

	#region Slamming
	void Slam() {
		slamForce = new(0, -50, 0);
	}
	#endregion

	#region Impulses
	void AddImpulse() {
		
	}
	#endregion
	// Handles Mouse Movement
	void HandleMouse() {
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

		transform.Rotate(Vector3.up * mouseX);

		currentXRotation -= mouseY;
		currentXRotation = Mathf.Clamp(currentXRotation, minY, maxY);

		playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
	}

	// Sets Movement Direction
	#endregion

}