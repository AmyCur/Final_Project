using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Magical;
using MathsAndSome;
using Globals;
using Elements;
using UnityEngine.InputSystem;

namespace Player {
	[RequireComponent(typeof(AudioSource))]
	public class PL_Controller : RB_Controller {



		bool shouldJump => canJump && Grounded() && magic.key.down(keys.jump);
		bool shouldSlide => slide.can && Grounded() && magic.key.down(keys.slide) && state != PlayerState.sliding;
		bool shouldDash => dash.can && magic.key.down(keys.dash) && stamina.s - dash.staminaPer >= 0;
		bool shouldSlam => slam.can && magic.key.down(keys.slam) && !Grounded() && state != PlayerState.slamming;

		[Header("Player")]
		[Header("States")]

		public PlayerState state;
		public MovementState jumpState = MovementState.none;
		public MovementState slideState = MovementState.none;
		public MovementState slamState = MovementState.none;
		public AdminState adminState = AdminState.standard;

		[Header("Movement")]
		public float forwardSpeed = 12f;
		public float sidewaysSpeed = 12f;


		[Header("Jumping")]
		public bool canJump;
		public float jumpForce;
		[SerializeField] float groundedRange = 1.08f;


		[Header("Slam")]

		public bool canSlam;

		[Header("Sliding")]

		public Force slide;
		// public bool canSlide = true;
		// public float slideSpeed = 45f;
		// [Range(500, 2000)] public int slideDecayIncrements = 30;
		// public float slideDecaySpeed = 10f;

		[Header("Dashing")]

		public Dash dash;

		// [Header("Mutual")]
		// public bool canDash = true;
		// public float dashHardCDTime = 0.1f;
		// [Range(0, 100)] public float staminaPerDash = 30f;

		// [Header("No Gravity")]
		// public float dashForce = 12f;
		// public float noGravDashTime = 0.3f;

		// [Header("Gravity")]

		// public float dashDecaySpeed = 10f;


		[Header("Mouse")]

		public float mouseSensitivityX;
		public float mouseSensitivityY;

		[SerializeField] float maxY = 90;
		[SerializeField] float minY = -90;

		float currentXRotation;



		[Header("Stamina")]

		public Stamina stamina;

		[Header("Slamming")]

		public Force slam;

		[HideInInspector] public Camera playerCamera;
		[SerializeField] new CapsuleCollider collider;
		float playerHeight => collider.height;
		//


		float hInp;
		float vInp;

		public GameObject forwardObject;

		[Header("UI")]

		public Cur.UI.HUDController hc;

		[Header("Admin")]

		[SerializeField] bool admin = true;


		public override void SetStartDefaults() {
			base.SetStartDefaults();

			stamina.pc = this;

			StartCoroutine(RegenerateStamina());
			playerCamera = Camera.main;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (state != PlayerState.sliding && state != PlayerState.slamming && canMove) {
				if (adminState == AdminState.standard) this.Move();
				else this.AdminMove();
			}
		}



		public override void Update() {
			base.Update();

			if (hc != null) hc.UpdateHeath();


			HandleMouse();

			if (Grounded() && state == PlayerState.slamming) state = PlayerState.walking;

			// Movement

			if (adminState != AdminState.noclip) {
				if (shouldJump) Jump();
				if (shouldSlide) StartCoroutine(Slide());
				if (shouldDash) Dash();
				if (shouldSlam) Slam();
			}


			if (magic.key.down(keys.noclip)) CheckForAdmin();
		}

		void CheckForAdmin() {
			if (admin) adminState = adminState == AdminState.standard ? AdminState.noclip : AdminState.standard;
		}


		//* Stamina

		public IEnumerator RegenerateStamina() {
			while (true) {
				if (stamina.s < stamina.max && state != PlayerState.sliding && rb.useGravity) {

					// If not moving and on the ground
					float movementMultiplier = Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && Grounded() ? 2 : 1;


					stamina.Add(stamina.staminaPerTick * (Grounded() ? 1.5f : 1) * movementMultiplier);
					if (stamina.s > stamina.max) stamina.s = stamina.max;
					yield return new WaitForSeconds(stamina.regenTime);
				}
				else yield return 0;
			}

		}

		//* Stamina

		public void Slam() {
			state = PlayerState.slamming;
			rb.linearVelocity = Vector3.zero;
			rb.AddForce(0, -slam.force * Consts.Multipliers.SLAM_MULTIPLIER * Time.deltaTime, 0);

		}

		public IEnumerator DecaySlide(float decaySpeed = -1f) {
			if (decaySpeed == -1f) decaySpeed = slide.decaySpeed;
			float force = slide.force;

			// idk if this needs to be a do while
			do {
				rb.AddForce(slide.direction * force * Consts.Multipliers.SLIDE_MULTIPLIER * Time.deltaTime);
				force = Mathf.Lerp(force, 0, Time.deltaTime * decaySpeed);
				// Debug.LogWarning(force);
				// ???? Couldve just used yield return 0
				yield return new WaitForSeconds(decaySpeed / (float) slide.decayIncrements);
			} while (force > 0.1f && !shouldSlide && !shouldSlam);
		}

		IEnumerator WaitForSlideJumpPreservation() {
			yield return new WaitForSeconds(0.1f);
			canBreakFromSlidePreservation = true;
		}

		bool canBreakFromSlidePreservation = false;

		public IEnumerator PreserveSlideJump(Vector3 direction) {
			canBreakFromSlidePreservation = false;

			StartCoroutine(WaitForSlideJumpPreservation());

			do {
				rb.AddForce(direction * slide.force * Consts.Multipliers.SLIDE_MULTIPLIER * Time.deltaTime * .8f);
				direction = slide.direction;
				yield return 0;

				// if ((Grounded() && canBreakFromSlidePreservation) || shouldDash) break;
			} while (!((Grounded() && canBreakFromSlidePreservation) || shouldDash || shouldSlam));

			if (Grounded()) StartCoroutine(DecaySlide(decaySpeed: slide.decaySpeed));
		}

		public IEnumerator Slide() {

			slide.direction = Directions.SlideDirection(this);
			Vector3 direction = slide.direction;

			state = PlayerState.sliding;

			do {
				rb.AddForce(direction * slide.force * Consts.Multipliers.SLIDE_MULTIPLIER * Time.deltaTime);
				yield return 0;
			} while (magic.key.gk(keys.slide) && !shouldJump && Grounded() && !shouldSlam);


			// Handle Slide End based on how the slide has ended

			// No Grounded -> Medium decay
			// if (!Grounded()) StartCoroutine(DecaySlide(direction: slideDirection, decaySpeed: 2f)); 

			// Jumped -> No decay
			if (shouldJump) { StartCoroutine(PreserveSlideJump(direction)); }

			// No longer pressing slide -> Fast decay
			else if (!magic.key.gk(keys.slide)) StartCoroutine(DecaySlide(decaySpeed: slide.decaySpeed));

			state = PlayerState.walking;
		}


		public void Jump() {
			rb.AddForce(0, jumpForce * Consts.Multipliers.JUMP_MULTIPLIER, 0);

		}

		public void Dash() {
			dash.state = MovementState.start;
			stamina.Subtract(dash.staminaPer);
			StartCoroutine(NoGravityDash());
		}

		void Set0G() => timerOver = true;


		bool timerOver = false;

		public IEnumerator DecayDash(float decaySpeed = -1f) {
			if (decaySpeed == -1f) decaySpeed = dash.decaySpeed;
			float force = dash.force;

			// idk if this needs to be a do while
			do {
				Vector3 forceToAdd = dash.direction * force * 1_000f * Time.deltaTime;
				// Debug.Log(forceToAdd);
				rb.AddForce(forceToAdd);
				force = Mathf.Lerp(force, 0, Time.deltaTime * decaySpeed);
				yield return 0;
			} while (force > 0.1f && !shouldDash && !shouldSlam);

			dash.state = MovementState.none;

		}

		bool canBreakFromGravityDash = false;

		IEnumerator WaitForGravityDash() {
			yield return new WaitForSeconds(0.1f);
			canBreakFromGravityDash = true;
		}


		public IEnumerator GravityDash() {
			canBreakFromGravityDash = false;

			StartCoroutine(WaitForGravityDash());


			do {
				rb.AddForce(dash.direction * dash.gravityDashForce * Consts.Multipliers.DASH_MULTIPLIER * Time.deltaTime);


				yield return 0;

				// if ((Grounded() && canBreakFromSlidePreservation) || shouldDash) break;

			} while (!((Grounded() && canBreakFromGravityDash) || shouldDash || shouldSlam));

			dash.state = MovementState.end;


			if (Grounded()) StartCoroutine(DecayDash(decaySpeed: slide.decaySpeed));

			dash.state = MovementState.none;

		}

		// Dash where gravity is disabled (The only dash if youre on the ground)
		public IEnumerator NoGravityDash() {
			timerOver = false;
			rb.useGravity = false;

			Invoke(nameof(Set0G), dash.noGravDashTime);

			dash.direction = Directions.DashDirection(this, true);

			rb.linearVelocity = Vector3.zero;
			while (!timerOver && !shouldSlam) {

				rb.AddForce(dash.direction * dash.force * Time.deltaTime * Consts.Multipliers.DASH_MULTIPLIER);

				yield return 0;
			}

			dash.state = MovementState.middle;


			rb.useGravity = true;

			if (!Grounded()) StartCoroutine(GravityDash());
			else StartCoroutine(DecayDash());
		}

		void SetAdminMode() {
			// ResetForces();
			// forwardSpeed = defaultSpeed * 2;
			// adminState = AdminState.noclip;
			transform.GetChild(0).GetComponent<CapsuleCollider>().isTrigger = true;
			rb.useGravity = false;
		}

		void SetStandardMode() {
			// forwardSpeed = defaultSpeed;
			// adminState = AdminState.standard;
			transform.GetChild(0).GetComponent<CapsuleCollider>().isTrigger = false;
			rb.useGravity = true;
		}

		public new void Move() {

			hInp = Grounded() ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
			vInp = Grounded() ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");



			Vector3 forward = forwardObject.transform.forward;
			Vector3 right = forwardObject.transform.right;

			float fwSpeed = forwardSpeed * 100f;
			float sdSpeed = sidewaysSpeed * 100f;

			float airChange = Consts.Movement.AirSpeedChange(Grounded());

			Vector3 force = (forward * vInp * (Grounded() ? fwSpeed : sdSpeed) * airChange) + (right * hInp * sdSpeed * airChange);

			slide.ChangeDirection(force.normalized);
			dash.ChangeDirection(force.normalized);

			rb.AddForce(force);
			// moveDirection = force.normalized;

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



		public bool Grounded() => Physics.Raycast(collider.transform.position, Vector3.down, transform.localScale.y * groundedRange);



		void HandleMouse() {
			float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
			float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;

			transform.Rotate(Vector3.up * mouseX);

			currentXRotation -= mouseY;
			currentXRotation = Mathf.Clamp(currentXRotation, minY, maxY);

			playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
		}



		void AdminMove() {
			hInp = Input.GetAxisRaw("Horizontal");
			vInp = Input.GetAxisRaw("Vertical");

			Vector3 right = playerCamera.transform.right;
			Vector3 forward = playerCamera.transform.forward;

			rb.linearVelocity = (forward * vInp + right * hInp).normalized * forwardSpeed;

			if (magic.key.gk(keys.jump)) rb.linearVelocity = new(rb.linearVelocity.x, 30, rb.linearVelocity.z);
			if (magic.key.gk(keys.dash)) rb.linearVelocity = new(rb.linearVelocity.x, -30, rb.linearVelocity.z);
			if (magic.key.down(keys.teleport)) {
				if (Physics.Raycast(transform.position, playerCamera.transform.forward, out RaycastHit hit, 1020f)) {
					transform.position = hit.point;
				}
			}
		}

		public override void Die() => Destroy(gameObject);

	}
}