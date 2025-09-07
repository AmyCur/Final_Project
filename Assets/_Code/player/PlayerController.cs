using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathsAndSome;
using UnityEngine;
using Magical;
using PlayerStates;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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

[RequireComponent(typeof(Rigidbody))]           // For movement
[RequireComponent(typeof(CapsuleCollider))]     // For Collision
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour {

	#region Variables

	readonly Vector2 checkScale = new(1f, 0.06f);

	void OnDrawGizmos() {
		Vector3 scale = gameObject.transform.localScale;
		Vector3 pos = gameObject.transform.position;

		Gizmos.DrawCube(
			new Vector3(pos.x, pos.y - scale.y - (checkScale.y / 2), pos.z),
			new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
		);
	}

	bool Grounded() {
		Vector3 scale = gameObject.transform.localScale;
		Vector3 pos = gameObject.transform.position;

		List<Collider> colliders = Physics.OverlapBox(
			new Vector3(pos.x, pos.y - scale.y - (checkScale.y / 2), pos.z),
			new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
		).ToList();

		colliders.Remove(GetComponent<CapsuleCollider>());

		foreach (Collider col in colliders) {
			Debug.Log($"Colldier: {col.name}");
		}


		if (colliders.Count() > 0)
			return true;
		return false;

	}


	bool shouldJump => canJump && magic.key.down(keys.jump) && Grounded() && s != state.sliding;
	
	bool shouldDash => canDash && magic.key.down(keys.dash) && s != state.sliding;
	bool shouldSlide => canSlide && magic.key.down(keys.slide) && Grounded();

	Rigidbody rb;

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

	public float speed;
	public bool canMove = true;

	float hInp;
	float vInp;

	[Header("Dash")]

	public float dashForce;
	public float dashCD;
	public bool canDash = true;
	[Space(10)]
	[SerializeField] float dashReductionIncrement = 0.1f;
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





	[Header("Objects")]
	[Header("Movement Objects")]

	public Camera playerCamera;
	[Space(10)]
	public GameObject forwardObject;


	#endregion

	#region Core Functions

	void Start() {
		SetStartDefaults();
	}

	void FixedUpdate() {
		if (adminState == AdminState.standard) {
			if (canMove) {
				Move();
			}
		}
		else {
			AdminMove();
		}

	}

	void AdminMove() {
		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		Vector3 right = playerCamera.transform.right;
		Vector3 forward = playerCamera.transform.forward;

		rb.linearVelocity = (forward * vInp + right * hInp).normalized * speed;
	}

	void Update() {
		if (canRotate) HandleMouse();

		if (adminState == AdminState.standard) {
			if (shouldJump) Jump();
			if (shouldDash) {
				dashDirection = DashDirection();
				StartCoroutine(Dash());
			}
			if (shouldSlide) {
				slideDirection = SlideDirection();
				StartCoroutine(Slide());
			}
		}
		else {
			if (magic.key.gk(keys.jump)) {
				rb.AddForce(0, jumpForce * 5, 0);
			}

			if (magic.key.gk(keys.dash)) {
				rb.AddForce(0, -jumpForce * 5, 0);
			}
		}

		if (magic.key.down(keys.noclip) && adminMode) {
			if (adminState == AdminState.standard) {
				adminState = AdminState.noclip;
				GetComponent<CapsuleCollider>().isTrigger = true;
				rb.useGravity = false;
			}
			else {
				adminState = AdminState.standard;
				GetComponent<CapsuleCollider>().isTrigger = false;
				rb.useGravity = true;
			}

			rb.linearVelocity = Vector3.zero;
		}
	}

	#endregion

	#region Other Functions


	void SetStartDefaults() {
		rb = GetComponent<Rigidbody>();
		playerCamera = Camera.main;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	#region Movement
	void Jump() {
		// rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
		rb.AddForce(new(0, jumpForce * 10, 0));
	}

	Vector3 WASDMovement() {
		if (s != state.sliding) return new(MoveDirection().x * speed, 0, MoveDirection().z * speed);

		return Vector3.zero;

	}

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


	IEnumerator ReduceDashForce() {
		while (dashForceMultiplier > 0) {
			dashForceMultiplier -= dashReductionIncrement <= 0 ? 0.1f : dashReductionIncrement;
			yield return new WaitForSeconds(dashReductionIncrementTime);
		}

		dashForceMultiplier = 0f;
	}

	IEnumerator ReduceSlideForce() {
		while (slideForceMultiplier > 0) {
			slideForceMultiplier -= slideReductionIncrement <= 0 ? 0.1f : slideReductionIncrement;
			yield return new WaitForSeconds(slideReductionIncrementTime);
		}

		slideForceMultiplier = 0f;
	}

	IEnumerator Dash() {
		canDash = false;
		Debug.Log("Should Dash");
		dashForceMultiplier = 1f;

		yield return new WaitForSeconds(0.2f);
		StartCoroutine(ReduceDashForce());

		yield return new WaitForSeconds(dashCD);
		canDash = true;
	}

	IEnumerator Slide() {

		s = state.sliding;
		canSlide = false;
		slideForceMultiplier = 1f;
		yield return new WaitUntil(() => magic.key.up(keys.slide));
		slideForceMultiplier = 0f;
		canSlide = true;
		s = state.walking;

	}

	Vector3 DashForce() {
		Vector3 direction = dashDirection;
		direction *= dashForce * dashForceMultiplier;
		if (s != state.sliding) return direction;
		return Vector3.zero;

	}

	Vector3 SlideForce() {
		Vector3 direction = slideDirection;
		direction *= slideForce * slideForceMultiplier;
		return direction;
	}

	void Move() {
		// I should have the player be moved by adding all the movement vectors
		Vector3 velocity = WASDMovement() + DashForce() + SlideForce();

		if (velocity.y == 0) {
			velocity = new(velocity.x, rb.linearVelocity.y, velocity.z);
		}

		rb.linearVelocity = velocity;
	}

	Vector3 MoveDirection() {
		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		Vector3 forward = forwardObject.transform.forward;
		Vector3 right = forwardObject.transform.right;

		return (forward * vInp + right * hInp).normalized;
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