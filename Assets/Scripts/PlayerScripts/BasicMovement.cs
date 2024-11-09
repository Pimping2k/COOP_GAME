using System;
using Components;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicMovement : NetworkBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private StaminaComponent _staminaComponent;


    private IA_Controls playerInputActions;
    private Vector3 velocity;
    private Transform cameraTransform;
    private bool isSprinting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        cameraTransform = camera.GetComponent<Transform>();
        playerInputActions = new IA_Controls();
        playerInputActions.PlayerControl.Sprint.performed += OnSprintStarted;
        playerInputActions.PlayerControl.Sprint.canceled += OnSprintStopped;
        playerInputActions.PlayerControl.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.PlayerControl.Disable();
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        isSprinting = true;
        _staminaComponent.StopRegeneration();
    }

    private void OnSprintStopped(InputAction.CallbackContext context)
    {
        isSprinting = false;
        _staminaComponent.StartRegeneration();
    }

    private void Update()
    {
        if (!IsOwner) return;

        MovePlayer();

        if (isSprinting)
        {
            _staminaComponent.DecreaseStamina(5f * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        MovePlayer();
    }

    private void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = cameraTransform.forward * moveVertical + cameraTransform.right * moveHorizontal;
        moveDirection.y = 0;
        moveDirection.Normalize();

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        velocity = moveDirection * currentSpeed;

        rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(velocity.x, rb.velocity.y, velocity.z),
            currentSpeed * Time.fixedDeltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10 * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}