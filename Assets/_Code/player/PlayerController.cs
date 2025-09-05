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

namespace PlayerStates
{
    public enum state
    {
        walking,
        sliding,
        slamming,
        wall_run,
        dead
    }

    public enum AdminState
    {
        standard,
        noclip
    }
}

[RequireComponent(typeof(Rigidbody))]           // For movement
[RequireComponent(typeof(CapsuleCollider))]     // For Collision
[RequireComponent(typeof(BoxCollider))]         // To Stop the player being launched
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{

    #region Variables

    readonly Vector2 checkScale = new(1f, 0.06f);

    void OnDrawGizmos()
    {
        Vector3 scale = gameObject.transform.localScale;
        Vector3 pos = gameObject.transform.position;

        Gizmos.DrawCube(
            new Vector3(pos.x, pos.y - scale.y - (checkScale.y/2), pos.z),
            new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
        );
    }

    bool grounded()
    {

        Vector3 scale = gameObject.transform.localScale;
        Vector3 pos = gameObject.transform.position;
        
        List<Collider> colliders = Physics.OverlapBox(
            new Vector3(pos.x, pos.y - scale.y - (checkScale.y/2), pos.z),
            new Vector3(scale.x * checkScale.x, checkScale.y, scale.z * checkScale.x)
        ).ToList();

        colliders.Remove(GetComponent<CapsuleCollider>());

        foreach (Collider col in colliders)
        {
            Debug.Log($"Colldier: {col.name}");
        }
        

        if (colliders.Count() > 0)
            return true;
        return false;

    }


    bool shouldJump => canJump && magic.key.down(keys.jump) && grounded();




    Rigidbody rb;

    [Header("Controls")]
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

    [Header("Objects")]
    [Header("Movement Objects")]

    public Camera playerCamera;
    [Space(10)]
    public GameObject forwardObject;


    #endregion

    #region Core Functions

    void Start()
    {
        SetStartDefaults();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
    }

    void Update()
    {
        if (canRotate) HandleMouse();

        if (shouldJump)
        {
            Jump();
        }
    }

    #endregion

    #region Other Functions
    

    void SetStartDefaults()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;   
    }

    #region Movement
    void Jump() {
        // rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        rb.AddForce(new(0, jumpForce, 0));
    }

    Vector3 WASDMovement()
    {
        return new(MoveDirection().x * speed, 0, MoveDirection().z * speed); 
    }

    void Move()
    {
        // I should have the player be moved by adding all the movement vectors
        Vector3 velocity = WASDMovement();

        if (velocity.y == 0)
        {
            velocity = new(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        rb.linearVelocity = velocity;
    }

    Vector3 MoveDirection()
    {
        hInp = Input.GetAxisRaw("Horizontal");
        vInp = Input.GetAxisRaw("Vertical");

        Vector3 forward = forwardObject.transform.forward;
        Vector3 right = forwardObject.transform.right;

        return (forward * vInp + right * hInp).normalized;
    }

    #endregion


    // Handles Mouse Movement
    void HandleMouse()
    {
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