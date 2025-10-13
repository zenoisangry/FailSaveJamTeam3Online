using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private InputAction m_jumpAction;

    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    private Rigidbody m_rigidbody;

    public float WalkSpeed = 5.0f;
    public float RotateSpeed = 5.0f;
    public float JumpSpeed = 5.0f;

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
        m_moveAction = playerMap.FindAction("Move");
        m_lookAction = playerMap.FindAction("Look");
        m_jumpAction = playerMap.FindAction("Jump");

        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_lookAction.ReadValue<Vector2>();

        if (m_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }

    public void Jump()
    {
        m_rigidbody.AddForce(Vector3.up * JumpSpeed, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        Walking();
        Rotating();
    }

    private void Walking()
    {
        Vector3 moveDirection = transform.forward * m_moveAmt.y + transform.right * m_moveAmt.x;
        moveDirection.Normalize();
        m_rigidbody.MovePosition(m_rigidbody.position + moveDirection * WalkSpeed * Time.deltaTime);
    }

    private void Rotating()
    {
        float rotationAmount = m_lookAmt.x * RotateSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
        m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
    }
}