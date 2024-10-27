using System;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField]private bool groundedPlayer;
    private Vector3 playerVelocity;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyMovement();
        ApplyGravityAndJump();
    }

    private void CheckGrounded()
    {
        //groundedPlayer = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
    }

    private void ApplyMovement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * (Time.deltaTime * playerSpeed));

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }
    }

    private void ApplyGravityAndJump()
    {
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
