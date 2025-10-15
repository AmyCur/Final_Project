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

	public enum DashState {
		just_dashed,
		dashing,
		not_dashing
	}

	public enum AdminState {
		standard,
		noclip
	}
}


[RequireComponent(typeof(AudioSource))]
public class PL_Controller : RB_Controller {

	readonly Vector2 checkScale = new(.3f, 0.06f);

	//TODO:
	//* Add dash and slide direction change
	//* Add slide momentum
	//* Add slide and dash jumping
	//* Polish dash force
	//* You can spam dash and go insanely fast (And slide but less so)
	//* Need to add a check to see if the player is oscilating between two points while sliding, and if they are increasing the slide direction until they stop oscilating


	Transform collider;

	[HideInInspector] public bool justDashed;
	[HideInInspector] public float dashForceMultiplier;

	enum SlidePosition
	{
		up,
		down
	}

	
	[Header("Objects")]
	public GameObject forwardObject;
	public Transform tallPos;
	public Transform shortPos;
	[SerializeField] HUDController hc;

	bool shouldJump => canJump && Grounded() && magic.key.down(keys.jump);
	bool shouldSlide => slide.can && magic.key.down(keys.slide) && Grounded() && state != PlayerState.sliding;
	bool shouldSlam => canSlam && magic.key.down(keys.slam) && !Grounded();
	bool shouldDash => dash.can && magic.key.down(keys.dash) && state != PlayerState.sliding && (stamina.s - staminaPerDash > stamina.min);
	bool shouldRegenStamina => stamina.s < stamina.max && !stamina.regenerating;

	[Header("States")]

	[HideInInspector][SerializeField] JumpState jState;                         // Unimplemented
	public PlayerState state = PlayerState.walking;
	[HideInInspector] public SlideState slideState = SlideState.not_sliding;        // Unimplemented - This may need to be implemented inorder to fix sliding spam
	public AdminState adminState = AdminState.standard;
	[HideInInspector] public DashState dashState = DashState.not_dashing;           // Unimplemented - This will likely not need to be implemented
	[SerializeField] SlidePosition slidePosition = SlidePosition.up;
	public bool adminMode = true;

	[Header("Camera")]

	[HideInInspector] public Camera playerCamera;
	float currentXRotation;
	[Min(0.1f)] public float mouseSensitivityX = 2f;
	[Min(0.1f)] public float mouseSensitivityY = 2f;
	[Range(-90, -60)][SerializeField] float minY = -90;
	[Range(60, 90)][SerializeField] float maxY = 90;
	[SerializeField] float cameraLerpSpeed = 50f;

	[Header("Movement")]
	public float forwardSpeed = 12f;
	public float sidewaysSpeed = 5f;

	public float maxSpeed = 13f;
	float minSpeed => -maxSpeed;


	float defaultSpeed;

	float hInp;
	float vInp;

	static Vector3 moveDirection;

	[Header("Jumping")]

	public float jumpForce = 32f;
	public float justJumpedTime = 0.1f;
	public bool canJump = true;
	public float groundedRange = 0.55f;

	[Header("Sliding")]

	public Force slide;
	public float playerHeight;
	Coroutine slideRoutine;

	[Header("Dashing")]

	public Force dash;
	public float dashEndForce;
	public float dashTime = 1f;

	[Header("Stamina")]

	public Stamina stamina;
	public float staminaPerDash = 30;
	public float groundedStaminaIncrease = 1.3f;

	[Header("Slamming")]

	public bool canSlam = true;
	public float slamForce = 90f;
	public float slamJumpForceMultiplier = 1.2f;

	float defaultJumpForce;
	float slamJumpForce => defaultJumpForce * slamJumpForceMultiplier;


	[Serializable]
	public class Stamina {
		public float min;
		public float max;
		public float s;
		public bool regenerating;
		public float regenTime;
		[HideInInspector] public PL_Controller pc;

		public IEnumerator RegenerateStamina() {
			regenerating = true;
			while (s < max) {
				yield return new WaitForSeconds(regenTime / (pc.Grounded() ? 1.3f : 1f));
				if (pc.state != PlayerState.sliding) s++;
			}

			if (s > max) s = max;
			regenerating = false;
		}

	}

	[Serializable]
	public class Force {

		public enum ForceState {
			start,
			middle,
			end
		}

		public ForceState forceState;
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

		public void ResetGoneBack() => this.goneBack = new bool[] { false, false };

		public void ResetDirection() => direction = Vector3.zero;
	}



	public override void SetStartDefaults() {
		base.SetStartDefaults();

		defaultJumpForce = jumpForce;

		slidePosition = SlidePosition.up;

		collider = transform.GetChild(0);
		playerHeight = collider.GetComponent<CapsuleCollider>().height;

		defaultSpeed = forwardSpeed;

		stamina.pc = this;

		playerCamera = Camera.main;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		if (state != PlayerState.sliding && state != PlayerState.slamming) this.Move();
	}

	public void ClampSpeed() => rb.linearVelocity = new Vector3(
		Mathf.Clamp(rb.linearVelocity.x, minSpeed, maxSpeed),
		Mathf.Clamp(rb.linearVelocity.y, -100, 30),
		Mathf.Clamp(rb.linearVelocity.z, minSpeed, maxSpeed)
	);

	void SetAdminMode() {
		ResetForces();
		forwardSpeed = defaultSpeed * 2;
		adminState = AdminState.noclip;
		transform.GetChild(0).GetComponent<CapsuleCollider>().isTrigger = true;
		rb.useGravity = false;
	}

	void SetStandardMode() {
		forwardSpeed = defaultSpeed;
		adminState = AdminState.standard;
		transform.GetChild(0).GetComponent<CapsuleCollider>().isTrigger = false;
		rb.useGravity = true;
	}



	public override void Update() {
		base.Update();
		HandleMouse();

		ClampSpeed();
		
		if(!!collider) Debug.DrawRay(collider.position, Vector3.down * groundedRange*collider.transform.localScale.y, Color.red);


		hc.UpdateStaminaBars();



		// If stamina isnt full, regenerate
		if (shouldRegenStamina) StartCoroutine(stamina.RegenerateStamina());


		if (Grounded()) {
			dash.ResetGoneBack();
			slide.ResetGoneBack();

			if (dash.forceState == Force.ForceState.middle) {
				dash.forceState = Force.ForceState.end;
			}
			if (slide.forceState == Force.ForceState.middle) slide.ResetDirection();
		}


		if (adminState == AdminState.standard) {
			if (shouldJump) Jump();
			if (shouldSlide) StartCoroutine(Slide());
			if (shouldSlam) StartCoroutine(Slam());
			if (shouldDash) StartCoroutine(Dash());
		}
		else {
			AdminMove();
		}

		if (magic.key.down(keys.noclip) && adminMode) {
			if (adminState == AdminState.standard) SetAdminMode();
			else SetStandardMode();
			rb.linearVelocity = Vector3.zero;
		}



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
		rb.AddForce(0, jumpForce * multiplier * 100, 0);
	}

	#endregion






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




	public IEnumerator ManageSlidePlayerHeight() {
		collider.GetComponent<CapsuleCollider>().height = playerHeight / 4;

		collider.transform.position = new(
			collider.transform.position.x,
			collider.transform.position.y - (playerHeight / (4 * 3 / transform.localScale.y)),
			collider.transform.position.z
		);

		yield return new WaitUntil(() => magic.key.up(keys.slide));

		collider.GetComponent<CapsuleCollider>().height = playerHeight;

		collider.transform.position = new(
			collider.transform.position.x,
			collider.transform.position.y + (playerHeight / (4 * 3 / transform.localScale.y)),
			collider.transform.position.z
		);
	}



	IEnumerator LerpSlideHeight(SlidePosition targetHeight) {

		Debug.Log(targetHeight);

		StartCoroutine(ManageSlidePlayerHeight());


		Dictionary<SlidePosition, Transform> SlideTrans = new() {
			{SlidePosition.up, tallPos},
			{SlidePosition.down, shortPos}
		};

		bool condition;

		do {
			condition = targetHeight == SlidePosition.up ? !shouldSlide : state == PlayerState.sliding;

			playerCamera.transform.localPosition = new(
				playerCamera.transform.localPosition.x,
				Mathf.Lerp(playerCamera.transform.localPosition.y, SlideTrans[targetHeight].localPosition.y, Time.deltaTime * cameraLerpSpeed),
				playerCamera.transform.localPosition.z
			);
			yield return 0;
		} while (playerCamera.transform.localPosition.y != SlideTrans[targetHeight].localPosition.y && condition);

	}

	public IEnumerator Slide() {

		state = PlayerState.sliding;

		if (slideRoutine != null) StopCoroutine(slideRoutine);
		slideRoutine = StartCoroutine(LerpSlideHeight(SlidePosition.down));

		slide.direction = SlideDirection();

		do {
			rb.AddForce(slide.direction * slide.force * 10000 * Time.deltaTime);
			yield return 0;
		} while (magic.key.gk(keys.slide) && !shouldJump && !shouldSlide);

		if (slideRoutine != null) StopCoroutine(slideRoutine);
		slideRoutine = StartCoroutine(LerpSlideHeight(SlidePosition.up));

		StartCoroutine(DecaySlide());


	}

	IEnumerator Slam() {
		rb.linearVelocity = Vector3.zero;
		rb.AddForce(0, -slamForce * 100, 0);
		state = PlayerState.slamming;

		while (!Grounded() && adminState == AdminState.standard) { yield return 0; }

		state = PlayerState.walking;
		jumpForce = slamJumpForce;
		yield return new WaitForSeconds(0.2f);
		jumpForce = defaultJumpForce;

	}

	void ResetForces() {
		dash.ResetDirection();
		slide.ResetDirection();
	}

	Vector3 DashDirection(bool increaseForward = true) {
		hInp = Input.GetAxisRaw("Horizontal");
		vInp = Input.GetAxisRaw("Vertical");

		Vector3 forward = forwardObject.transform.forward;
		Vector3 right = forwardObject.transform.right;


		if (hInp != 0) return vInp != 0 ? (forward * vInp + right * hInp).normalized : (right * hInp).normalized;
		else return vInp != 0 ? (forward * vInp).normalized : increaseForward ? forward * 1.1f : forward /*Forward is increased because it feels smaller cos ur not moving*/;
	}


	public IEnumerator HandleDashState() {
		yield return new WaitForSeconds(0.4f);
		dash.forceState = Force.ForceState.middle;
	}


	public IEnumerator DashFirstPart() {

		ResetForces();

		stamina.s -= staminaPerDash;

		rb.useGravity = false;

		rb.linearVelocity = new(
			rb.linearVelocity.x,
			rb.linearVelocity.y / 10f,
			rb.linearVelocity.z
		);

		dash.direction = DashDirection();

		dash.forceState = Force.ForceState.start;
		dashState = DashState.just_dashed;

		float df = dash.force;

		for (int i = 0; i < Mathf.FloorToInt(dashTime * 100); i++) {
			rb.AddForce(new(dash.direction.x * df * 100, 0, dash.direction.z * df * 100));
			yield return new WaitForSeconds(0.01f);
		}

		rb.useGravity = true;

		dash.forceState = Force.ForceState.middle;

		StartCoroutine(DashEnd());
	}

	public IEnumerator DashEnd(float decaySpeed = 15f) {
		float df = dashEndForce;

		do
		{
			rb.AddForce(new Vector3(dash.direction.x * df * (Grounded() ? 1 : .3f) * 10_000, 0, dash.direction.z * df * (Grounded() ? 1 : .3f) * 10_000) * Time.deltaTime);
			if (Grounded()) df = Mathf.Lerp(df, 0, Time.deltaTime * decaySpeed * (dash.forceState == Force.ForceState.end ? 5 : 1));
			yield return 0;

			// // Cancels the dash if another dash is started (Waits atleast one frame so that the dash is not instantly cancelled)
			// if (shouldDash) break;
		} while (df > 0f && !shouldDash);
		
		dashState = DashState.not_dashing;
	}



	public IEnumerator Dash(float decaySpeed = 4f) {

		// ResetForces();
		// StartCoroutine(HandleDashState());

		// dash.forceState = Force.ForceState.started;

		// rb.linearVelocity = Vector3.zero;
		//  stamina.s -= staminaPerDash;


		// float df = dash.force;
		// dash.direction = DashDirection();
		// do {
		// 	rb.AddForce(new(dash.direction.x * df * (Grounded() ? 1 : .3f) * 100, 0, dash.direction.z * df * (Grounded() ? 1 : .3f) * 100));
		// 	if (Grounded()) df = Mathf.Lerp(df, 0, Time.deltaTime * decaySpeed * (dash.forceState == Force.ForceState.ending ? 5 : 1));
		// 	yield return 0;

		// 	// // Cancels the dash if another dash is started (Waits atleast one frame so that the dash is not instantly cancelled)
		// 	// if (shouldDash) break;
		// } while (df > 0f && !shouldDash);

		StartCoroutine(DashFirstPart());
		yield return 0;
	}

	void OnDrawGizmos()
	{
        // Vector3 scale = transform.localScale;
        // Vector3 pos = transform.position;
        // Gizmos.DrawCube(
        // 	new Vector3(pos.x, pos.y - ((scale.y - (checkScale.y / 2)) / (state == PlayerState.sliding ? 1.15f : 1)), pos.z),
        // 	new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
        // );

        // if (!!collider)
        // {
		// 	Gizmos.DrawRay(collider.position, collider.transform.position + (Vector3.down) * 1.5f);
        // }
	}

	public bool Grounded() {
		return Physics.Raycast(collider.position, Vector3.down, transform.localScale.y * groundedRange);
		
		// Vector3 scale = transform.localScale;
		// Vector3 pos = transform.position;

		// List<Collider> colliders = Physics.OverlapBox(
		// 	new Vector3(pos.x, pos.y - ((scale.y - (checkScale.y / 2)) / (state == PlayerState.sliding ? 1.15f : 1)), pos.z),
		// 	new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
		// ).ToList();

		// colliders.Remove(transform.GetChild(0).GetComponent<CapsuleCollider>());
		
		// foreach (Collider c in colliders.ToList()) {
		// 	if (c.isTrigger) colliders.Remove(c);
		// 	if (glob.isEntity(c.tag)) colliders.Remove(c);
		// }

		// if (colliders.Count() > 0)
		// 	return true;
		// return false;
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

		if (magic.key.gk(keys.jump)) rb.linearVelocity = new(rb.linearVelocity.x, 30, rb.linearVelocity.z);
		if (magic.key.gk(keys.dash)) rb.linearVelocity = new(rb.linearVelocity.x, -30, rb.linearVelocity.z);
		if (magic.key.down(keys.teleport)) {
			if (Physics.Raycast(transform.position, playerCamera.transform.forward, out RaycastHit hit, 1000f)) {
				transform.position = hit.point;
			}
		}
	}
	#endregion
}