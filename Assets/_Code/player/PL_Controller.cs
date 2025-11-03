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



		bool shouldJump => 	canJump  && Grounded() && magic.key.down(keys.jump);
		bool shouldSlide => canSlide && Grounded() && magic.key.gk(keys.slide) && state != PlayerState.sliding;
		bool shouldDash =>  canDash  && magic.key.down(keys.dash) && stamina.s - staminaPerDash >= 0;


		[Header("Movement")]
		public float 					forwardSpeed 		= 12f;
		public float 					sidewaysSpeed 		= 12f;

		
		[Header("Jumping")]
		public bool canJump;
		public float 					jumpForce;
		[SerializeField] float 			groundedRange 		= 1.08f;


		[Header("Slam")]

		public bool canSlam;

		[Header("Sliding")]

		public bool canSlide = true;
		public float slideSpeed = 45f;

		[Header("Dashing")]
		
		[Header("Mutual")]
		public bool canDash = true;
		public float dashHardCDTime = 0.1f;
		[Range(0, 100)] public float staminaPerDash = 30f;

		[Header("No Gravity")]
		public float dashForce = 12f;
		public float noGravDashTime = 0.3f;
		
		
		[Header("Mouse")]

		public float 					mouseSensitivityX;
		public float 					mouseSensitivityY;
		
		[SerializeField] float 			maxY				= 90;
		[SerializeField] float 			minY 				= -90;
		
		float 							currentXRotation;

		[Header("States")]

		public PlayerState 				state;
		public MovementState 			jumpState 			= MovementState.none;
		public MovementState 			slideState 			= MovementState.none;
		public MovementState 			slamState 			= MovementState.none;

		[Header("Stamina")]

		public Stamina stamina;
		


		[HideInInspector] public Camera playerCamera;
		[SerializeField] CapsuleCollider collider;
		float 							playerHeight => collider.height;
		//
		
		
		float hInp;
		float vInp;
	
		public GameObject forwardObject;


		public override void SetStartDefaults() {
			base.SetStartDefaults();

		
			playerCamera = Camera.main;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (state != PlayerState.sliding && state != PlayerState.slamming && canMove) this.Move();
		}

		public override void Update() {
			base.Update();
			HandleMouse();

			// Movement

			if (shouldJump) Jump();
			if (shouldSlide) StartCoroutine(Slide());
			if (shouldDash) Dash();
		}
		
		public IEnumerator Slide() {

			Vector3 slideDirection = Directions.SlideDirection(this);

			state = PlayerState.sliding;

			do {
				rb.AddForce(slideDirection * slideSpeed * 10000f * Time.deltaTime);
				yield return 0;
			} while (magic.key.gk(keys.slide) && !shouldJump && Grounded());

			state = PlayerState.walking;

			// Handle Slide End based on how the slide has ended

			// No longer pressing slide -> Fast decay
			if (!magic.key.gk(keys.slide)) { }

			// Jumped -> No decay
			else if (shouldJump) { }

			// No Grounded -> Medium decay
			else if (!Grounded()) { }


        }

		public void Jump() {
			rb.AddForce(0, jumpForce * 1000f, 0);
		}

		public void Dash() => StartCoroutine(NoGravityDash());

		

		void Set0G() => timerOver = true;


		bool timerOver = false;

		public IEnumerator GravityDash() {
			yield return 0;
        }

		// Dash where gravity is disabled (The only dash if youre on the ground)
		public IEnumerator NoGravityDash() {
			timerOver = false;
			rb.useGravity = false;

			Invoke(nameof(Set0G), noGravDashTime);

			Vector3 dashDirection = Directions.DashDirection(this, true);
			rb.linearVelocity = Vector3.zero;
			while (!timerOver) {
				rb.AddForce(dashDirection*dashForce);
				yield return 0;
			}
			
			rb.useGravity = true;

			if (!Grounded()) StartCoroutine(GravityDash());
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
				if (Physics.Raycast(transform.position, playerCamera.transform.forward, out RaycastHit hit, 1000f)) {
					transform.position = hit.point;
				}
			}
		}
		
	}
}