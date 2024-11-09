using System;
using Components;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private StaminaComponent _staminaComponent;
    private IA_Controls playerInputActions;

    private bool isSprinting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
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
            _staminaComponent.DecreaseStamina(1f * Time.deltaTime);
        }
    }

    private void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveHorizontal, 0, moveVertical).normalized * speed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}