using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private Transform cam;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, -0.9f, 0f);
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameManager.InputDisabled) return;
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    void FixedUpdate()
    {
        if (GameManager.InputDisabled) return;
        Move();

    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v);

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * inputDir.z + right * inputDir.x;

        Vector3 velocity = moveDir * speed;

        rb.linearVelocity = new Vector3(
            velocity.x,
            rb.linearVelocity.y,
            velocity.z
        );
    }

    private void Jump()
    {
        if (!IsGrounded()) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        Vector3 pos = transform.position + groundCheckOffset;

        return Physics.CheckSphere(
            pos,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}