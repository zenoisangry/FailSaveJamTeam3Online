using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset InputActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction gravityAction;
    private InputAction pauseActionPlayer;
    private InputAction pauseActionUI;

    [Header("References")]
    public Transform cameraHolder;
    private Rigidbody rb;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float rotationSpeed = 120f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 1.0f;
    public float controllerSensitivity = 2.0f;
    public float maxLookAngle = 80f;

    [Header("Gravity Settings")]
    public float gravityStrength = 9.81f;
    public float gravityCheckDistance = 3f;
    public float rotateToSurfaceSpeed = 5f;

    [Header("Menu Settings")]
    public GameObject pauseDisplay;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 gravityDirection = Vector3.down;
    private float cameraPitch = 0f;
    private bool isRotatingToSurface = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        var playerMap = InputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        gravityAction = playerMap.FindAction("ChangeGravity");

        pauseActionPlayer = InputSystem.actions.FindAction("Player/Pause");
        pauseActionUI = InputSystem.actions.FindAction("UI/Pause");
    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Update()
    {
        moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        lookInput = lookAction?.ReadValue<Vector2>() ?? Vector2.zero;

        if (gravityAction != null && gravityAction.WasPressedThisFrame() && !isRotatingToSurface)
            StartCoroutine(ChangeGravityToClosestSurface());

        DisplayPause();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        MovePlayer();
        HandleCameraRotation();
    }

    // ---------------------------------------------------------
    // MOVIMENTO
    // ---------------------------------------------------------

    private void MovePlayer()
    {
        if (isRotatingToSurface) return;

        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        Vector3 targetVelocity = moveDirection * walkSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - Vector3.ProjectOnPlane(currentVelocity, -gravityDirection);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    // ---------------------------------------------------------
    // CAMERA E ROTAZIONE
    // ---------------------------------------------------------

    private void HandleCameraRotation()
    {
        if (isRotatingToSurface) return;

        float sensitivity = (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
            ? mouseSensitivity
            : controllerSensitivity;

        float yaw = lookInput.x * rotationSpeed * sensitivity * Time.deltaTime;
        float pitch = -lookInput.y * rotationSpeed * sensitivity * Time.deltaTime;

        Quaternion yawRotation = Quaternion.AngleAxis(yaw, -gravityDirection);
        transform.rotation = yawRotation * transform.rotation;

        cameraPitch = Mathf.Clamp(cameraPitch + pitch, -maxLookAngle, maxLookAngle);
        Quaternion pitchRotation = Quaternion.AngleAxis(cameraPitch, cameraHolder.right);

        Quaternion alignToGravity = Quaternion.FromToRotation(cameraHolder.up, -gravityDirection) * cameraHolder.rotation;

        cameraHolder.rotation = Quaternion.Slerp(
            cameraHolder.rotation,
            pitchRotation * transform.rotation,
            Time.deltaTime * 15f
        );
    }

    // ---------------------------------------------------------
    // GRAVITÀ
    // ---------------------------------------------------------

    private void ApplyGravity()
    {
        rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
    }

    private IEnumerator ChangeGravityToClosestSurface()
    {
        isRotatingToSurface = true;

        Vector3[] directions =
        {
            transform.up, -transform.up, transform.right, -transform.right, transform.forward, -transform.forward
        };

        Vector3 closestNormal = -gravityDirection;
        float closestDist = Mathf.Infinity;

        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, gravityCheckDistance))
            {
                if (hit.distance < closestDist)
                {
                    closestDist = hit.distance;
                    closestNormal = hit.normal;
                }
            }
        }

        if (closestDist == Mathf.Infinity)
        {
            isRotatingToSurface = false;
            yield break;
        }

        Vector3 newGravity = -closestNormal.normalized;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.FromToRotation(-gravityDirection, closestNormal) * transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotateToSurfaceSpeed;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            cameraHolder.rotation = Quaternion.Slerp(cameraHolder.rotation, targetRot, t);
            yield return null;
        }

        gravityDirection = newGravity;
        isRotatingToSurface = false;
    }

    // ---------------------------------------------------------
    // MENU OPZIONI
    // ---------------------------------------------------------

    private void DisplayPause()
    {
        if (pauseActionPlayer.WasPressedThisFrame())
        {
            pauseDisplay.SetActive(true);
            InputActions.FindActionMap("Player").Disable();
            InputActions.FindActionMap("UI").Enable();
        }
        else if (pauseActionUI.WasPressedThisFrame())
        {
            pauseDisplay.SetActive(false);
            InputActions.FindActionMap("Player").Enable();
            InputActions.FindActionMap("UI").Disable();
        }
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
    }

    public void SetControllerSensitivity(float value)
    {
        controllerSensitivity = value;
    }
}