using Unity.Netcode;
using UnityEngine;

public class BasicMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField]private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        MovePlayer();
    }

    private void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveHorizontal, 0, moveVertical).normalized * speed * Time.deltaTime;

        // Перемещение через Rigidbody
        rb.MovePosition(rb.position + move);

        // Прыжок
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}