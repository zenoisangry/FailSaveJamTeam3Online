using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input System")]
    public InputActionAsset InputActions;

    [Header("References")]
    public Transform cameraHolder; // assegna il transform del "CameraHolder" nel prefab

    [Header("Movement Settings")]
    public float WalkSpeed = 5.0f;
    public float RotateSpeed = 5.0f;
    public float JumpSpeed = 5.0f;
    public float GravityStrength = 9.81f;

    [Header("Gravity Detection")]
    public float gravityCheckRadius = 2f;
    public float gravityRayLength = 5f;
    public LayerMask gravitySurfaces;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction changeGravityAction;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 gravityDirection = Vector3.down;
    private bool isRotating = false;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        var playerMap = InputActions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        jumpAction = playerMap.FindAction("Jump");
        changeGravityAction = playerMap.FindAction("FlipGravity"); // mappata su C

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
            Jump();

        if (changeGravityAction.WasPressedThisFrame() && !isRotating)
            TryChangeGravity();

        HandleCameraRotation();
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();
        Move();
    }

    private void Move()
    {
        Vector3 moveDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        rb.MovePosition(rb.position + moveDir * WalkSpeed * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        rb.AddForce(-gravityDirection * JumpSpeed, ForceMode.Impulse);
    }

    private void ApplyCustomGravity()
    {
        rb.AddForce(gravityDirection * GravityStrength, ForceMode.Acceleration);
    }

    private void TryChangeGravity()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, gravityCheckRadius, gravitySurfaces);

        Transform closestSurface = null;
        float closestDistance = float.MaxValue;
        Vector3 surfaceNormal = Vector3.up;

        foreach (Collider hit in hits)
        {
            Vector3 closestPoint = hit.ClosestPoint(transform.position);
            float distance = Vector3.Distance(transform.position, closestPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;

                // Ottieni la normale con un raycast verso quella direzione
                if (Physics.Raycast(transform.position, (closestPoint - transform.position).normalized,
                    out RaycastHit hitInfo, gravityRayLength, gravitySurfaces))
                {
                    closestSurface = hit.transform;
                    surfaceNormal = hitInfo.normal;
                }
            }
        }

        if (closestSurface != null)
        {
            StartCoroutine(RotateToSurface(surfaceNormal));
        }
    }

    private IEnumerator RotateToSurface(Vector3 surfaceNormal)
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        Quaternion cameraStart = cameraHolder.rotation;
        Quaternion cameraTarget = Quaternion.FromToRotation(cameraHolder.up, surfaceNormal) * cameraHolder.rotation;

        Vector3 newGravityDirection = -surfaceNormal;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            cameraHolder.rotation = Quaternion.Slerp(cameraStart, cameraTarget, t);
            yield return null;
        }

        gravityDirection = newGravityDirection.normalized;
        isRotating = false;
    }

    private void HandleCameraRotation()
    {
        // Rotazione orizzontale del player
        float yaw = lookInput.x * RotateSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0, Space.Self);

        // Rotazione verticale della camera
        float pitch = -lookInput.y * RotateSpeed * Time.deltaTime;
        cameraHolder.Rotate(pitch, 0, 0, Space.Self);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, gravityCheckRadius);
    }
}