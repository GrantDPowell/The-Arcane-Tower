using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementSystem : MonoBehaviour
{
    public float sprintMultiplier = 1.5f;
    private bool isSprinting;
    public float baseMoveSpeed = 2.0f;
    public float turnSpeed = 200.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;

    private float moveSpeed;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Animator animator;

    private PlayerInput playerInput;
    private Vector2 moveInput;

    private PlayerCameraSystem cameraController; // Reference to the CameraController script

    public PlayerStats playerStats; // Reference to the PlayerStats scriptable object

    private Quaternion movRot;

    private InputAction sprintAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraController = FindObjectOfType<PlayerCameraSystem>(); // Find the CameraController script in the scene

        sprintAction = playerInput.actions["Sprint"];

        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        // Subscribe to input actions
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        playerInput.actions["Jump"].performed += OnJump;
        sprintAction.performed += OnSprint;
        sprintAction.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        // Unsubscribe from input actions to avoid memory leaks
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
        playerInput.actions["Jump"].performed -= OnJump;
        sprintAction.performed -= OnSprint;
        sprintAction.canceled -= OnSprintCanceled;
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            animator.SetBool("isJumping", true); // Update animator to indicate player is jumping
        }
    }

    void Update()
    {
        UpdateMoveSpeed(); // Update the movement speed dynamically

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            animator.SetBool("isJumping", false); // Update animator to indicate player is not jumping
        }

        // Handle movement
        float currentMoveSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        Vector3 move = transform.forward * moveInput.y * currentMoveSpeed * Time.deltaTime;
        Vector3 strafe = transform.right * moveInput.x * currentMoveSpeed * Time.deltaTime;
        Vector3 movement = move + strafe;

        if (cameraController.currentStyle == PlayerCameraSystem.CameraStyle.Topdown)
        {
            // Make movement relative to the camera
            Transform cameraTransform = Camera.main.transform;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Flatten the camera vectors
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            movement = (cameraForward * moveInput.y + cameraRight * moveInput.x) * currentMoveSpeed * Time.deltaTime;

            if (moveInput != Vector2.zero)
            {
                controller.Move(movement);
                RotateTowardsMouse();
            }
            else {
                RotateTowardsMouse();
            }
        }
        else
        {
            controller.Move(movement);
        }

        if (cameraController.currentStyle == PlayerCameraSystem.CameraStyle.Basic)
        {
            if (moveInput.x != 0)
            {
                float turn = moveInput.x * turnSpeed * Time.deltaTime;
                transform.Rotate(0, turn, 0);
            }
        }

        // Update the Speed parameter in the Animator
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }


    private void RotateTowardsMouse()
    {
        Vector3 mousePosition = cameraController.GetMouseWorldPosition();
        if (mousePosition != Vector3.zero)
        {
            Vector3 direction = (mousePosition - transform.position).normalized;
            direction.y = 0; // Keep the character rotation on the horizontal plane
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    private void UpdateMoveSpeed()
    {
        moveSpeed = baseMoveSpeed + playerStats.GetMoveSpeed();
        //Debug.Log($"Updated Move Speed: {moveSpeed}");
    }

}
