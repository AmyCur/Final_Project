using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Magical;
using Entity;
using UI;
using UI.HUD;
using System.Linq;
using Combat.Enemies;
using MathsAndSome;

namespace Player.Movement {
	[RequireComponent(typeof(AudioSource))]
	public class PL_Controller : RB_Controller {


		public bool shouldJump => canJump && BoxGrounded(justSlid ? 2f : 1.3f) && magic.key.down(keys.jump);

		public bool shouldSlide => slide.can && Grounded(1.3f) && magic.key.down(keys.slide) && state != PlayerState.sliding;
		
		public bool shouldDash => dash.can && magic.key.down(keys.dash) && stamina.s - dash.staminaPer >= 0;
		
		public bool shouldSlam => slam.can && magic.key.down(keys.slam) && !Grounded() && state != PlayerState.slamming;

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

		[HideInInspector] public bool justSlid=false;

		public Force slide;
		
		public GameObject slideCameraObject;
		
		public GameObject defaultCameraObject;

		[Header("Dashing")]

		public DashForce dash;

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
		
		public new CapsuleCollider collider;
		
		[SerializeField] CapsuleCollider footCollider;
		
		float playerHeight => collider.height;

		float hInp;
		
		float vInp;

		[HideInInspector] public Vector3 fw => (transform.forward + Camera.main.transform.forward).normalized;
		
		[HideInInspector] public Vector3 cameraPos => playerCamera.transform.position;

		[Header("Slope")]

		public float maxSlopeAngle=30f;
		
		RaycastHit slopeHit;

		public GameObject forwardObject;

		[Header("Friction")]

		[SerializeField] float defaultFriction;
		
		[SerializeField] float slopeFriction=0.5f;

		[SerializeField] float dashFriction=0f;

		[Header("UI")]

		public HUDController hc;

		[Header("Admin")]

		public bool admin = true;
		
		[SerializeField] float adminSpeed = 40f;

		public GameObject EnemySpawnScreen;

		#region Start
		void Awake(){
			MathsAndSome.mas.player.Player=this;
			NotificationManager.mb=this;
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
            if(Input.GetKeyDown(KeyCode.LeftBracket)) Entity.Entity.KillAll(typeof(ENM_Controller));
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

		void ClampVerticalVelocity(){
			if(rb!=null){
				rb.linearVelocity=new Vector3(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -1000, maxVerticalSpeed), rb.linearVelocity.z);
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

			SetSlopeFriction();

			ClampVerticalVelocity();

			if (state != PlayerState.sliding && state != PlayerState.slamming && canMove) {
				if (adminState == AdminState.noclip) this.AdminMove();	
			}

			Audio.ChangeAudioOnIntensity.FadeSongBasedOnDangerLevel();

			SetDashFriction();

			print(Dash.dashForceToAdd);
		}

		void SetDashFriction(){
			if(dash.state!=MovementState.none){
				footCollider.SetFriction(dashFriction);
			}
		}


		public override void FixedUpdate() {
			// ba3se.FixedUpdate();

			if (state != PlayerState.sliding && state != PlayerState.slamming && canMove) {
				if (adminState == AdminState.standard) this.Move();
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
				if (shouldDash) Dash.HandleDash();
				if (shouldSlide) StartCoroutine(Slide.SlideRoutine());
				else if (shouldSlam) Slam.HandleSlam();
            }
		}

		IEnumerator HandleSlamMomentumPreservation(){
			finalSlamVelocity=rb.linearVelocity.y;
			yield return new WaitForSeconds(0.3f);
			finalSlamVelocity=0f;
		}

		

		public void ClampIfRampTooSteep(){
			if(Physics.Raycast(transform.position-new Vector3(0f,transform.localScale.y/2f,0f), transform.forward, out RaycastHit hit, 1f)){
				if (!hit.isProjectile()){
					// Debug.Log(hit.normal);
					float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
					if(angle>maxSlopeAngle && angle < 90f){
						// Debug.Log("Thats high");
						rb.linearVelocity=new Vector3(rb.linearVelocity.x,Mathf.Clamp(rb.linearVelocity.y, -1_000f, 0f),rb.linearVelocity.z);
					}
				}
			}
		}

		void CheckForAdmin() {
			if (admin){
				adminState = adminState == AdminState.standard ? AdminState.noclip : AdminState.standard;
				rb.useGravity = adminState != AdminState.noclip;
				if(adminState==AdminState.noclip) state=PlayerState.walking;

				foreach(CapsuleCollider col in GetComponentsInChildren(typeof(CapsuleCollider))){
					col.isTrigger = adminState == AdminState.noclip;
				}
				stamina.s=stamina.max;
				NotificationManager.AddNotification($"Noclip: {adminState==AdminState.noclip}");
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

		bool moving;

		public new void Move() {

			hInp = Grounded() ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
			vInp = Grounded() ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

			moving = hInp!=0f || vInp!=0f;

			Vector3 forward = forwardObject.transform.forward;
			Vector3 right = forwardObject.transform.right;

			float fwSpeed = forwardSpeed * 100f;
			float sdSpeed = sidewaysSpeed * 100f;

			float airChange = Consts.Movement.AirSpeedChange(Grounded());

			Vector3 force = (forward * vInp * fwSpeed * airChange) + (right * hInp * sdSpeed * airChange);
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
				if(!col.isEntity<PL_Controller>() && !col.isProjectile()) {
					// Debug.Log(col.name);
					return true;
				}
			}
			return false;
		}

		public void SetSlopeFriction(){
			if(Physics.Raycast(footCollider.transform.position, Vector3.down, out slopeHit, 0.6f, ~playerMask) || Physics.Raycast(footCollider.transform.position, MathsAndSome.mas.vector.MultiplyVectors(new List<Vector3>(){Vector3.forward,moveDirection.normalized}), out slopeHit, 1f, ~playerMask)){
				if(!slopeHit.isProjectile()){
					float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
					if(moving || state!=PlayerState.walking){
						if(angle!=0f) footCollider.SetFriction(slopeFriction);
						else footCollider.SetFriction(defaultFriction);
					}
					else footCollider.SetFriction(defaultFriction);
					
				}

				return;
			}
			
			footCollider.SetFriction(defaultFriction);
		}

		public bool OnSlope(){
			if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + .5f)){
				if(!slopeFriction.isProjectile()){
					float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
					return angle < maxSlopeAngle && angle!=0;
				}
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

			rb.linearVelocity = (forward * vInp + right * hInp).normalized * adminSpeed * Time.deltaTime;

			if (magic.key.gk(keys.jump)) rb.linearVelocity = new(rb.linearVelocity.x, 30, rb.linearVelocity.z);
			if (magic.key.gk(keys.dash)) rb.linearVelocity = new(rb.linearVelocity.x, -30, rb.linearVelocity.z);
			if (magic.key.down(keys.teleport)) {
				RaycastHit[] hits = Physics.RaycastAll(transform.position, forward, 1020f);
				if (hits.Length>0) {
					foreach(RaycastHit hit in hits){
						if(!hit.isEntity<PL_Controller>()){
							transform.position = hit.point;
							Debug.Log(hit.transform.name);
						}
					}
				}
			}
		}

		public override void Die() { 
			state=PlayerState.dead;
		}
	}
}