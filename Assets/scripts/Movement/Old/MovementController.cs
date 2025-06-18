using UnityEngine;

public class MovementController : MonoBehaviour
{
    InputInterface input = new StandardInput();

    public InputInterface Input => input;

    [Header("Movement")]
    public float moveSpeed;
    public float moveSpeedLimit;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundCheckLayer;

    float horizontalInput;
    float verticalInput;
    bool isJumping = false;
    bool isGrounded;

    Vector3 moveDirection;
    Rigidbody rigidBody;

    void Awake()
    {
        var ghostInput = gameObject.GetComponent<GhostInput>();
        if (ghostInput)
        {
            input = ghostInput;
        }
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;

        readyToJump = true;
    }

    void Update()
    {
        // Check if player is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 1.2f, groundCheckLayer);

        // Set drag based on grounded state (using drag instead of linearDamping)
        if (isGrounded)
        {
            rigidBody.linearDamping = groundDrag;
        }
        else
        {
            rigidBody.linearDamping = 0f;
        }
        MyInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();

        if (!isGrounded)
        {
            float fallMultiplier = 4f;
            rigidBody.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            print(rigidBody.linearVelocity.y);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 2);
    }

    void MyInput()
    {
        horizontalInput = input.GetMoveHorizontal();
        verticalInput = input.GetMoveVertical();
        isJumping = input.IsJumpPressed() && readyToJump && isGrounded;

        if (isJumping)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (isGrounded)
        {
            rigidBody.AddForce(moveDirection * moveSpeed, ForceMode.Force);
        }
        else
        {
            rigidBody.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        // Only control horizontal velocity
        Vector3 flatVel = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);

        // Limit speed
        if (flatVel.magnitude > moveSpeedLimit)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeedLimit;
            rigidBody.linearVelocity = new Vector3(limitedVel.x, rigidBody.linearVelocity.y, limitedVel.z);
        }
        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, rigidBody.linearVelocity.y, rigidBody.linearVelocity.z);
    }

    void Jump()
    {
        Vector3 currentVelocity = rigidBody.linearVelocity;
        currentVelocity.y = 0f;
        rigidBody.linearVelocity = currentVelocity;

        Vector3 jumpDirection = (Vector3.up).normalized;

        // Add jump force
        rigidBody.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }
}
