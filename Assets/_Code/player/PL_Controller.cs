using System.Collections;
using UnityEngine;
using Magical;
using EntityLib;
using Cur.UI;

// Note: Weird jittering doesnt happen in build!?

namespace Player {
	public class PL_Controller : RB_Controller
    {
		#region Junk
		// This is rubbish
		// public bool Grounded()=>true;
		public Camera playerCamera => Camera.main;
		[Header("Junk")]
		public bool canSlam;
		public float forwardSpeed;
		public float sidewaysSpeed;
		public HUDController hc;
		public GameObject EnemySpawnScreen;
		public Vector3 cameraPos;
		public Stamina stamina;
		public GameObject forwardObject;
		public Vector3 fw;
		#endregion

		public bool shouldJump => magic.key.down(keys.jump) && canJump && Grounded();

		[Header("Camera")]
		public float mouseSensitivityX;
		public float mouseSensitivityY;
		public GameObject eyes;

		[Header("Movement")]

		Vector3 forwards => transform.forward;
		Vector3 right => transform.right;

		[Header("Jumping")]

		public bool canJump=true;
		public float jumpForce=45f;

		public bool Grounded(float m = 1f){
			Vector3 startPos = transform.position-new Vector3(0,transform.localScale.y/2f,0);
			Collider[] colliders = Physics.OverlapBox(startPos, new Vector3(transform.localScale.x*.2f,0.5f*m*1.1f,transform.localScale.z*.2f));
			foreach(Collider col in colliders){
				Debug.Log(col.name);
				if(!col.isEntity(typeof(PL_Controller))) {
					return true;
				}
			}
			return false;
		}

		float currentXRotation;
		const float minY=-90;
		const float maxY=90;


		[System.Serializable]
		public class LerpVector{
			public Vector3 current;
			public Vector3 target;
			public Vector3 currentVelocity;
			public float smoothTime;
			public Vector3 finalVector(){
				current = Vector3.SmoothDamp(current,target,ref currentVelocity, smoothTime);
				return current;
			}
		}

		public LerpVector movementLerpVector;
		public LerpVector globalLerpVector;
		public LerpVector jumpLerpVector;


        public void HandleMouse()
        {
			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

			transform.Rotate(Vector3.up * mouseX);

			currentXRotation -= mouseY;
			currentXRotation = Mathf.Clamp(currentXRotation, minY, maxY);

			playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
		}


		public void HandleMovement(){
			float hInp = Input.GetAxisRaw("Horizontal");
			float vInp = Input.GetAxisRaw("Vertical");


			Vector3 moveSpeed = (vInp*forwards*forwardSpeed + hInp*right*sidewaysSpeed)*1000f;
			
			movementLerpVector.target=moveSpeed;
			movementLerpVector.finalVector();
		}

		public IEnumerator HandleJump(){
			Debug.Log("Jump");
			
			jumpLerpVector.target = Vector3.zero;
			jumpLerpVector.current=new Vector3(0,jumpForce*Time.deltaTime*80f,0);
			jumpLerpVector.smoothTime=1f;
			do{
				jumpLerpVector.finalVector();
				yield return 0;
			}while(!shouldJump);
		}

		public override void Update()
        {
            HandleMouse();
			HandleMovement();

			Debug.Log($"{magic.key.down(keys.jump)} && {canJump} && {Grounded()}");
			
			if(shouldJump) StartCoroutine(HandleJump());


			globalLerpVector.target=movementLerpVector.finalVector();
			rb.AddForce(globalLerpVector.finalVector()+jumpLerpVector.finalVector()*Time.deltaTime*80f);
		}

		public override void Start(){
			base.Start();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
    }
}