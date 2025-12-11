using System.Collections;
using UnityEngine;
using Magical;
using EntityLib;
using Cur.UI;

namespace Player {
	[RequireComponent(typeof(AudioSource))]
	public class PL_Controller : RB_Controller {

		bool shouldJump => canJump && BoxGrounded(justSlid ? 2f : 1.3f) && magic.key.down(keys.jump);
		bool shouldSlide => slide.can && Grounded(1.3f) && magic.key.down(keys.slide) && state != PlayerState.sliding;
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
		public float maxSpeed = 35f;
		public float maxVerticalSpeed = 35f;
		public LayerMask playerMask;


		[Header("Jumping")]
		public bool canJump;
		public float jumpForce;
		[SerializeField] float groundedRange = 1.08f;
		public float trueGroundedRange (float m = 1f) => transform.localScale.y * groundedRange * m;

		[Header("Slam")]

		public bool canSlam;
		[SerializeField] float finalSlamVelocity;
		[SerializeField] float slamCheckRange=2f;

		[Header("Sliding")]

		public Force slide;

		[Header("Dashing")]

		public Dash dash;

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

		[HideInInspector] public Vector3 fw => (transform.forward + Camera.main.transform.forward).normalized;
		[HideInInspector] public Vector3 cameraPos => playerCamera.transform.position;

		[Header("Slope")]

		public float maxSlopeAngle=30f;
		RaycastHit slopeHit;

		public GameObject forwardObject;

		[Header("UI")]

		public Cur.UI.HUDController hc;

		[Header("Admin")]

		[SerializeField] bool admin = true;

		public GameObject EnemySpawnScreen;

		#region Start
		void Awake(){
			MathsAndSome.mas.player.Player=this;
		}

		public override void SetStartDefaults() {
			base.SetStartDefaults();

			stamina.pc = this;

			StartCoroutine(RegenerateStamina());
			playerCamera = Camera.main;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		#endregion

		#region Update

		void AdminFunctionality()
        {
            if(Input.GetKeyDown(KeyCode.LeftBracket)) EntityLib.Entity.KillAll(typeof(ENM_Controller));
			if (magic.key.down(keys.noclip)) CheckForAdmin();
        }

		void HandleIFrames()
        {
            if (health.takenDamage) {
				health.takenDamage = false;
				StartCoroutine(IFrames());
			}
        }

		void HandleSlamEnding()
        {
            if (BoxGrounded(1.3f) && state == PlayerState.slamming) state = PlayerState.walking;
			if (BoxGrounded(slamCheckRange) && state == PlayerState.slamming) StartCoroutine(HandleSlamMomentumPreservation());
        }

		void HardClampVelocity()
        {
            if(rb!=null){
				rb.linearVelocity = MathsAndSome.mas.vector.ClampVector(rb.linearVelocity, new Vector3[]
					{
						new Vector3(-1_000,-1_000,-1_000),
						new Vector3(1000, jumpState==MovementState.none ? 5 : 15, 1000)
					}
				);
			}
        }

		public override void Update() {

			base.Update();

			//* Movement
			HandleMove();
			HandleMouse();
			HardClampVelocity();
			HandleSlamEnding();

			//* Slope Movement
			HandleSlope();
			ClampIfRampTooSteep();

			//* Admin Movement
			AdminFunctionality();

			//* Combat
			HandleIFrames();
			if (hc != null) hc.UpdateHeath();			
		}

		public override void FixedUpdate() {
			// base.FixedUpdate();

			if (state != PlayerState.sliding && state != PlayerState.slamming && canMove) {
				if (adminState == AdminState.standard) this.Move();
				else this.AdminMove();
			}
			
			Vector2 hVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
			if(hVel.magnitude>maxSpeed) hVel=hVel.normalized*maxSpeed;
			
			rb.linearVelocity = new Vector3(hVel.x, rb.linearVelocity.y, hVel.y);
			// rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, 0, maxSpeed), rb.linearVelocity.y, Mathf.Clamp(rb.linearVelocity.z,0,maxSpeed));
		}
		#endregion

		IEnumerator IFrames() {
			health.canTakeDamage = false;
			yield return new WaitForSeconds(0.2f);
			health.canTakeDamage = true;
		}

		void HandleSlope(){
			if(OnSlope() && jumpState!=MovementState.middle){
				if(state != PlayerState.sliding){
					transform.position=new Vector3(slopeHit.point.x,slopeHit.point.y+transform.localScale.y,slopeHit.point.z);
					if(adminState != AdminState.noclip) rb.useGravity=false;
				}
			}
			else{
				if(adminState != AdminState.noclip) rb.useGravity=true;
			}
		}

		public void HandleMove(){
            if (adminState != AdminState.noclip)
            {
                if (shouldJump) Jump();
				if (shouldDash) Dash();
				if (shouldSlide) StartCoroutine(Slide());
				else if (shouldSlam) Slam();
            }
		}

		IEnumerator HandleSlamMomentumPreservation(){
			finalSlamVelocity=rb.linearVelocity.y;
			yield return new WaitForSeconds(0.3f);
			finalSlamVelocity=0f;
		}

		

		public void ClampIfRampTooSteep(){
			if(Physics.Raycast(transform.position-new Vector3(0f,transform.localScale.y/2f,0f), transform.forward, out RaycastHit hit, 1f)){
				Debug.Log(hit.normal);
				float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
				if(angle>maxSlopeAngle && angle < 90f){
					Debug.Log("Thats high");
					rb.linearVelocity=new Vector3(rb.linearVelocity.x,Mathf.Clamp(rb.linearVelocity.y, -1_000f, 0f),rb.linearVelocity.z);
				}

			}
		}
		void CheckForAdmin() {
			if (admin){
				adminState = adminState == AdminState.standard ? AdminState.noclip : AdminState.standard;
				rb.useGravity = adminState != AdminState.noclip;

				foreach(CapsuleCollider col in GetComponentsInChildren(typeof(CapsuleCollider))){
					col.isTrigger = adminState == AdminState.noclip;
				}
				stamina.s=stamina.max;
			}
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
			rb.AddForce(new Vector3(0, -slam.force *100f * Time.deltaTime, 0));
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
			} while (force > 0.1f && state != PlayerState.slamming);
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
			} while (!((Grounded() && canBreakFromSlidePreservation) || shouldDash || state == PlayerState.slamming || adminState == AdminState.noclip));

			if (Grounded()) StartCoroutine(DecaySlide(decaySpeed: slide.decaySpeed));
		}
		
		public GameObject slideCameraObject;
		public GameObject defaultCameraObject;
		Coroutine cameraSlideRoutine;

		public IEnumerator LerpCameraSlide(bool down){
			
			while(playerCamera.transform.position != (down ? slideCameraObject.transform.position : defaultCameraObject.transform.position)){
				Vector3 pos = playerCamera.transform.position;
				playerCamera.transform.position = new Vector3(
					pos.x,
					Mathf.Lerp(pos.y, down ? slideCameraObject.transform.position.y : defaultCameraObject.transform.position.y, Time.deltaTime*20f),
					pos.z
				);
				yield return 0;
			}



		}


		public IEnumerator Slide() {

			justSlid=false;

			collider.height=1;

			slide.direction = Directions.SlideDirection(this);
			Vector3 direction = slide.direction;

			state = PlayerState.sliding;

			if(cameraSlideRoutine!=null) StopCoroutine(cameraSlideRoutine);
			cameraSlideRoutine = StartCoroutine(LerpCameraSlide(true));

			do {
				rb.AddForce(direction * (slide.force+finalSlamVelocity) * Consts.Multipliers.SLIDE_MULTIPLIER * Time.deltaTime);
				yield return 0;
			} while (magic.key.gk(keys.slide) && !shouldJump  && state != PlayerState.slamming && adminState != AdminState.noclip);


			if(cameraSlideRoutine!=null) StopCoroutine(cameraSlideRoutine);
			cameraSlideRoutine = StartCoroutine(LerpCameraSlide(false));

			// Handle Slide End based on how the slide has ended

			// No Grounded -> Medium decay
			// if (!Grounded()) StartCoroutine(DecaySlide(direction: slideDirection, decaySpeed: 2f)); 

			// Jumped -> No decay
			if (shouldJump) { StartCoroutine(PreserveSlideJump(direction)); }
			else if (!Grounded(1.3f)) StartCoroutine(PreserveSlideJump(direction));

			// No longer pressing slide -> Fast decay
			else if (!magic.key.gk(keys.slide)) StartCoroutine(DecaySlide(decaySpeed: slide.decaySpeed));

			state = PlayerState.walking;
			justSlid=true;
			collider.height=2;
			yield return new WaitForSeconds(0.2f);
			justSlid=false;

		}

		bool justSlid=false;


		IEnumerator WaitForJumpEnd(){
			yield return new WaitForSeconds(0.1f);
			yield return new WaitUntil(() => Grounded());
			jumpState=MovementState.none;
		}

		public void Jump() {
			jumpState=MovementState.middle;
			rb.AddForce(0, jumpForce * Consts.Multipliers.JUMP_MULTIPLIER, 0);
			StartCoroutine(WaitForJumpEnd());


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
			} while (force > 0.1f && !shouldDash && state != PlayerState.slamming && adminState != AdminState.noclip);

			dash.state = MovementState.none;

		}

		bool canBreakFromGravityDash = false;

		IEnumerator WaitForGravityDash() {
			yield return new WaitForSeconds(0.1f);
			canBreakFromGravityDash = true;
		}


		public IEnumerator GravityDash() {
			rb.linearVelocity=new Vector3(0,rb.linearVelocity.y,0);
			canBreakFromGravityDash = false;

			StartCoroutine(WaitForGravityDash());


			do {
				rb.AddForce(dash.direction * dash.gravityDashForce * Consts.Multipliers.DASH_MULTIPLIER * Time.deltaTime, ForceMode.Acceleration);


				yield return 0;

				// if ((Grounded() && canBreakFromSlidePreservation) || shouldDash) break;

			} while (!((Grounded() && canBreakFromGravityDash) || shouldDash || state == PlayerState.slamming || adminState == AdminState.noclip));

			dash.state = MovementState.end;


			if (Grounded()) StartCoroutine(DecayDash(decaySpeed: slide.decaySpeed));

			dash.state = MovementState.none;

		}
		bool noGravTimerOver = false;

		IEnumerator SetNoGravTimerOver(){
			noGravTimerOver=false;
			yield return new WaitForSeconds(dash.noGravDashTime);
			noGravTimerOver=true;
		}

		IEnumerator DashCD(){
			dash.can = false;
			yield return new WaitForSeconds(0.2f);
			dash.can = true;
		}

		// Dash where gravity is disabled (The only dash if youre on the ground)
		public IEnumerator NoGravityDash() {
			rb.useGravity = false;
			yield return 0;
			rb.linearVelocity=Vector3.zero;

			StartCoroutine(SetNoGravTimerOver());

			dash.direction = Directions.DashDirection(this, true);

			rb.linearVelocity = Vector3.zero;
			while (!noGravTimerOver && state != PlayerState.slamming && adminState != AdminState.noclip) {

				rb.AddForce(dash.direction * dash.force * Time.deltaTime * Consts.Multipliers.DASH_MULTIPLIER, ForceMode.Acceleration);

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

		Vector3 moveDirection;

		public new void Move() {

			hInp = Grounded() ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
			vInp = Grounded() ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

			Vector3 forward = forwardObject.transform.forward;
			Vector3 right = forwardObject.transform.right;

			float fwSpeed = forwardSpeed * 100f;
			float sdSpeed = sidewaysSpeed * 100f;

			float airChange = Consts.Movement.AirSpeedChange(Grounded());

			Vector3 force = (forward * vInp * (Grounded() ? fwSpeed : sdSpeed) * airChange) + (right * hInp * sdSpeed * airChange);
			moveDirection=force;

			slide.ChangeDirection(force.normalized);
			dash.ChangeDirection(force.normalized);

			rb.AddForce(force);

			if(force!=Vector3.zero){

			}
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



		public bool Grounded(float m = 1f) => Physics.Raycast(collider.transform.position, Vector3.down, transform.localScale.y * groundedRange*m, ~playerMask);
		public bool BoxGrounded(float m = 1f){
			Vector3 startPos = transform.position-new Vector3(0,transform.localScale.y/2f,0);
			Collider[] colliders = Physics.OverlapBox(startPos, new Vector3(transform.localScale.x*.2f,0.5f*m,transform.localScale.z*.2f));
			foreach(Collider col in colliders){
				if(!col.isEntity(typeof(PL_Controller))) {
					// Debug.Log(col.name);
					return true;}
			}
			return false;
		}

		public bool OnSlope(){
			if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + .3f)){
				float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
				return angle < maxSlopeAngle && angle!=0;
			}
			return false;
		}

		Vector3 GetSlopeMoveDirection(){
			return Vector3.ProjectOnPlane(moveDirection.normalized, slopeHit.normal).normalized;
		}



		void HandleMouse() {
			if(!Props.inMenu){
				float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
				float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;

				transform.Rotate(Vector3.up * mouseX);

				currentXRotation -= mouseY;
				currentXRotation = Mathf.Clamp(currentXRotation, minY, maxY);

				if(playerCamera==null) playerCamera=Camera.main;
				playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);

			}
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