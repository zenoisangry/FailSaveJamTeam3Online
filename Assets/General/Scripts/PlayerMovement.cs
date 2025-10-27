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
    public AudioSource switchSound;
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
    public float airAttachDelay = 1.0f;

    [Header("Head Sway Settings")]
    public float swayAmplitude = 2f;
    public float swayFrequency = 2f;
    public float moveSwaySpeed = 3f;
    public float baseCameraY = 0.5f;
    public float minCameraY = 0.3f;

    [Header("Menu Settings")]
    public GameObject pauseDisplay;
    public GameObject settingsDisplay;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 gravityDirection = Vector3.down;
    private float cameraPitch = 0f;
    private bool isRotatingToSurface = false;
    private float airTime = 0f;

    public AudioSource steps;

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

        pauseDisplay.SetActive(false);
        settingsDisplay.SetActive(false);
    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
        InputActions.FindAction("Player/Archive").Disable();
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

        if (!IsGrounded())
        {
            if (airTime != -0.1f)
            {
                airTime += Time.deltaTime;
                if (airTime >= airAttachDelay && !isRotatingToSurface)
                {
                    float tempGravityCheck = gravityCheckDistance;
                    gravityCheckDistance = 10;
                    StartCoroutine(ChangeGravityToClosestSurface());
                    gravityCheckDistance = tempGravityCheck;
                    airTime = -0.1f;
                }
            }
        }
        else
        {
            airTime = 0f;
        }

        if (steps.isPlaying)
        {
            if (moveInput == Vector2.zero || !IsGrounded())
            {
                steps.Stop();
            }
        }
        else
        {
            if (moveInput != Vector2.zero && IsGrounded())
            {
                steps.pitch = Random.Range(0.95f, 1.05f);
                steps.Play();
            }
        }

        HandleCameraRotation();
        HandleCameraSway();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        MovePlayer();
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

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, gravityDirection, out _, 1.1f);
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

        cameraHolder.rotation = Quaternion.Slerp(
            cameraHolder.rotation,
            pitchRotation * transform.rotation,
            Time.deltaTime * 15f
        );
    }

    private void HandleCameraSway()
    {
        if (isRotatingToSurface) return;

        float swayPhase = Mathf.Abs(Mathf.Sin(Time.time * moveSwaySpeed)) * moveInput.magnitude;
        float targetY = Mathf.Lerp(baseCameraY, minCameraY, swayPhase);

        Vector3 targetPos = new Vector3(0, targetY, 0);
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetPos, Time.deltaTime * 5f);
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
            transform.up, transform.right, -transform.right, transform.forward, -transform.forward
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
        switchSound.Play();
        Vector3 newGravity = -closestNormal.normalized;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.FromToRotation(-gravityDirection, closestNormal) * transform.rotation;

        float t = 0f;
        
        while (t < 1f)
        {
            t += Time.deltaTime * rotateToSurfaceSpeed;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            float swayOffset = Mathf.Sin(t * Mathf.PI * swayFrequency) * swayAmplitude;
            Quaternion swayRotation = Quaternion.AngleAxis(swayOffset, transform.forward);
            cameraHolder.rotation = Quaternion.Slerp(cameraHolder.rotation, targetRot, Time.deltaTime) * swayRotation;

            yield return null;
        }

        gravityDirection = newGravity;
        isRotatingToSurface = false;
    }

    // ---------------------------------------------------------
    // SENSIBILITÀ
    // ---------------------------------------------------------

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
    }

    public void SetControllerSensitivity(float value)
    {
        controllerSensitivity = value;
    }
}