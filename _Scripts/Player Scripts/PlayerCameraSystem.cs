using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraSystem : MonoBehaviour
{

    [Header("Edge Rotation Settings")]
    public float edgeThreshold = 50f; // Pixels from the edge of the screen
    public float edgeRotationSpeed = 100f; // Speed of rotation when near the edge

    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;

    public CameraStyle currentStyle;
    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
    }

    private void OnEnable()
    {
        playerInput.actions["SwitchToBasic"].performed += ctx => SwitchCameraStyle(CameraStyle.Basic);
        playerInput.actions["SwitchToCombat"].performed += ctx => SwitchCameraStyle(CameraStyle.Combat);
        playerInput.actions["SwitchToTopdown"].performed += ctx => SwitchCameraStyle(CameraStyle.Topdown);
    }

    private void OnDisable()
    {
        playerInput.actions["SwitchToBasic"].performed -= ctx => SwitchCameraStyle(CameraStyle.Basic);
        playerInput.actions["SwitchToCombat"].performed -= ctx => SwitchCameraStyle(CameraStyle.Combat);
        playerInput.actions["SwitchToTopdown"].performed -= ctx => SwitchCameraStyle(CameraStyle.Topdown);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SwitchCameraStyle(CameraStyle.Topdown); 
    }

    private void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        if (currentStyle == CameraStyle.Combat)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            Quaternion camRot = Camera.main.transform.rotation;
            camRot.x = 0;
            camRot.z = 0;
            transform.rotation = camRot;
        }
        else if (currentStyle == CameraStyle.Basic)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (currentStyle == CameraStyle.Topdown)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            HandleTopDownLook();
            //RotateCameraOnEdge();
        }
    }

    private void RotateCameraOnEdge()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (mousePosition.x <= edgeThreshold)
        {
            Camera.main.transform.Rotate(Vector3.up, -edgeRotationSpeed * Time.deltaTime);
        }
        else if (mousePosition.x >= screenWidth - edgeThreshold)
        {
            transform.Rotate(Vector3.up, edgeRotationSpeed * Time.deltaTime);
        }
    }


    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
        if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

        currentStyle = newStyle;
    }
    private void HandleTopDownLook()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 lookAtPoint = hitInfo.point;
            playerObj.LookAt(new Vector3(lookAtPoint.x, playerObj.position.y, lookAtPoint.z));
        }
    }
    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }
}
