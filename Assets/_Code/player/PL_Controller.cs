using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Magical;
using PlayerStates;
using MathsAndSome;
using Globals;
using Elements;
using UnityEngine.InputSystem;

namespace PlayerStates {
	public enum PlayerState {
		walking,
		sliding,
		slamming,
		crawling,
		dead
	}

	public enum JumpState {
		just_jumped,
		jumped,
		not_jumped
	}

	public enum SlideState {
		just_started_slide,
		sliding,
		just_ended_slide,
		not_sliding
	}

	public enum AdminState {
		standard,
		noclip
	}
}


[RequireComponent(typeof(CapsuleCollider))]     // For Collision
[RequireComponent(typeof(AudioSource))]
public class PL_Controller : RB_Controller {

	readonly Vector2 checkScale = new(.3f, 0.06f);

	//TODO:
	//* Add dash and slide direction change
	//* Add slide momentum
	//* Add slide and dash jumping
	//* Polish dash force
	//* You can spam dash and go insanely fast



	public GameObject forwardObject;
	[HideInInspector] public bool justDashed;
	[HideInInspector] public float dashForceMultiplier;
	

	bool shouldJump => canJump && Grounded() && magic.key.down(keys.jump);
	bool shouldSlide => slide.can && magic.key.down(keys.slide) && Grounded() && state != PlayerState.sliding;
	bool shouldSlam => canSlam && magic.key.down(keys.slam) && !Grounded();
	bool shouldDash => dash.can && magic.key.down(keys.dash) && state != PlayerState.sliding && (stamina.stamina - 30 > stamina.min);

	[Header("States")]

	[SerializeField] JumpState jState;
	public PlayerState state = PlayerState.walking; 
	public SlideState slideState = SlideState.not_sliding;
	public AdminState adminState = AdminState.standard;
	public bool adminMode = true;

	[Header("Camera")]

	[HideInInspector] public Camera playerCamera;
	float currentXRotation;
	[Min(0.1f)] public float mouseSensitivityX = 2f;
	[Min(0.1f)] public float mouseSensitivityY = 2f;
	[Range(-90, -60)] [SerializeField] float minY = -90;
	[Range(60, 90)]   [SerializeField] float maxY = 90;

	[Header("Movement")]
	public float forwardSpeed = 12f;
	public float sidewaysSpeed = 5f;
	float defaultSpeed;

	float hInp;
	float vInp;

	[Header("Jumping")]

	public float jumpForce = 32f;
	public float justJumpedTime = 0.1f;
	public bool canJump = true;

	[Header("Sliding")]


	[Header("Slamming")]

	public bool canSlam=true;
	public float slamForce = 50f;



	[System.Serializable]
	public class Force {
		public bool can = true;
		public float force;
		public float directionChangeSpeed;
		public Vector3 direction;
		public bool[] goneBack = new bool[2];

		public void DirectionChange() {
			Vector3 md = new(Math.Sign(moveDirection.x), 0, Math.Sign(moveDirection.z));
			Vector3 dd = new(Math.Sign(this.direction.x), 0, Math.Sign(this.direction.z));

			// Check if the player is moving in the opposite direciton of the dash and if they are, change the dash direction to suit them

			if (-md.x == dd.x && md.x != 0 && dd.x != 0) {
				if (this.direction.x < 0) this.direction = new(this.direction.x + this.directionChangeSpeed, this.direction.y, this.direction.z);
				else if (this.direction.x > 0) this.direction = new(this.direction.x - this.directionChangeSpeed, this.direction.y, this.direction.z);
				goneBack[0] = true;
			}

			if (-md.z == dd.z && md.z != 0 && dd.z != 0) {
				if (this.direction.z < 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z + this.directionChangeSpeed);
				else if (this.direction.z > 0) this.direction = new(this.direction.x, this.direction.y, this.direction.z - this.directionChangeSpeed);
				goneBack[1] = true;
			}

			if (goneBack[0]) this.direction = new(Mathf.Clamp(this.direction.x, -.8f, .8f), this.direction.y, this.direction.z);
			if (goneBack[1]) this.direction = new(this.direction.x, this.direction.y, Mathf.Clamp(this.direction.z, -.8f, .8f));
		}
	}

	public Force dash;
	public Force slide;

	static Vector3 moveDirection;


	[Serializable]
	public class Stamina {
		public float min;
		public float max;
		public float stamina;
		public bool regenerating;
		public float regenTime;

		public IEnumerator RegenerateStamina() {
			regenerating = true;
			while (stamina < max) {
				yield return new WaitForSeconds(regenTime);
				stamina++;
			}

			if (stamina > max) stamina = max;
			regenerating = false;
		}

	}

	public Stamina stamina;

	public override void SetStartDefaults() {
		base.SetStartDefaults();

		defaultSpeed = forwardSpeed;

		playerCamera = Camera.main;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		if (state != PlayerState.sliding && state != PlayerState.slamming) this.Move();
	}

	public override void Update() {
		base.Update();
		HandleMouse();

		rb.linearVelocity = new Vector3(
			Mathf.Clamp(rb.linearVelocity.x, -20, 20),
			Mathf.Clamp(rb.linearVelocity.y, -100, 30),
			Mathf.Clamp(rb.linearVelocity.z, -20, 20)
		);

		if (stamina.stamina < stamina.max && !stamina.regenerating) StartCoroutine(stamina.RegenerateStamina());


		if (Grounded()) {
			dash.goneBack = new bool[] { false, false };
			slide.goneBack = new bool[] { false, false };
		}


		if (adminState == AdminState.standard) {
			if (shouldJump) Jump();
			if (shouldSlide) StartCoroutine(Slide());
			if (shouldSlam) Slam();
			if (shouldDash) StartCoroutine(Dash());
		}
		else {
			AdminMove();
		}

		if (magic.key.down(keys.noclip) && adminMode) {
			if (adminState == AdminState.standard) {
				ResetForces();
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
		

		Debug.LogWarning($"{slide.can}  {Grounded()}  {magic.key.down(keys.slide)}");
	}

	#region Movement

	public new void Move() {

		dash.DirectionChange();
		slide.DirectionChange();

		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		Vector3 forward = forwardObject.transform.forward;
		Vector3 right = forwardObject.transform.right;

		Vector3 force = (forward * vInp * (forwardSpeed * 100)) + (right * hInp * (sidewaysSpeed * 100));

		rb.AddForce(force);
		moveDirection = force.normalized;

	}

	#endregion

	#region Jumping

	IEnumerator JustJumpedRoutine() {
		yield return new WaitForSeconds(justJumpedTime);
		jState = JumpState.jumped;
	}

	public void Jump(float multiplier = 1) {
		jState = JumpState.just_jumped;
		StartCoroutine(JustJumpedRoutine());
		rb.AddForce(0, jumpForce*multiplier*100, 0);
	}

	#endregion

	


	Vector3 DashDirection(bool increaseForward = true) {
		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		Vector3 forward = forwardObject.transform.forward;
		Vector3 right = forwardObject.transform.right;

		
		if (hInp != 0) return vInp != 0 ? (forward * vInp + right * hInp).normalized : (right * hInp).normalized;
		else return vInp != 0 ? (forward * vInp).normalized : increaseForward ? forward * 1.1f : forward /*Forward is increased because it feels smaller cos ur not moving*/;
	}

	Vector3 SlideDirection(bool increaseForward = false) => DashDirection(increaseForward);


	public IEnumerator DecaySlide(float decaySpeed = 7f) {

		state = PlayerState.walking;

		float sf = slide.force;

		while (sf > 1f && !shouldSlide) {
			rb.AddForce(slide.direction * sf * 10000 * Time.deltaTime);
			if (Grounded()) sf = Mathf.Lerp(sf, 0, Time.deltaTime * decaySpeed);
			yield return 0;
		}

	}

	public IEnumerator Slide() {

		state = PlayerState.sliding;
		Debug.Log("Slide start");

		slide.direction = SlideDirection();

		while (magic.key.gk(keys.slide) && !shouldJump && !shouldSlide) {
			Debug.Log("Sliding");
			rb.AddForce(slide.direction * slide.force * 10000 * Time.deltaTime);
			yield return 0;
		}

		Debug.Log("Slide end");
		StartCoroutine(DecaySlide());
	}

	void Slam() => rb.AddForce(0, -slamForce*100, 0);

	void ResetForces() {
		dash.direction = Vector3.zero;
		slide.direction = Vector3.zero;
	}


	public IEnumerator Dash(float decaySpeed = 7f) {

		ResetForces();
		stamina.stamina -= 30f;


		float df = dash.force;
		dash.direction = DashDirection();
		while (df > 0f) {
			rb.AddForce(new(dash.direction.x * df * (Grounded() ? 1 : .5f) * 100, 0, dash.direction.z * df * (Grounded() ? 1 : .5f) * 100));
			if (Grounded()) df = Mathf.Lerp(df, 0, Time.deltaTime * decaySpeed);
			yield return 0;
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
	
	#region Mouse
	void HandleMouse() {
		float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
		float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;

		transform.Rotate(Vector3.up * mouseX);

		currentXRotation -= mouseY;
		currentXRotation = Mathf.Clamp(currentXRotation, minY, maxY);

		playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
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
}