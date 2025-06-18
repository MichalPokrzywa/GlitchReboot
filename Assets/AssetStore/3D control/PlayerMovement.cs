using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    InputInterface input = new StandardInput();

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

    bool isGrounded;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    private bool mounted;

    Vector3 moveDirection;

    Rigidbody rigidBody;

    static bool isJumping = false;
    public static bool IsJumping => isJumping;

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

    private void MyInput()
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

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            rigidBody.AddForce(moveDirection * moveSpeed, ForceMode.Force);
        }
        else
        {
            rigidBody.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        // Only control horizontal velocity
        Vector3 flatVel = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);

        if (flatVel.magnitude > moveSpeedLimit)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeedLimit;
            rigidBody.linearVelocity = new Vector3(limitedVel.x, rigidBody.linearVelocity.y, limitedVel.z);
        }

        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, rigidBody.linearVelocity.y, rigidBody.linearVelocity.z);
    }

    private void Jump()
    {
        // Zachowujemy poziom� pr�dko��, resetujemy tylko pionow� (aby skok by� bardziej kontrolowany)
        Vector3 currentVelocity = rigidBody.linearVelocity;
        currentVelocity.y = 0f;
        rigidBody.linearVelocity = currentVelocity;

        // Tworzymy kierunek skoku jako kombinacj� wierzcho�kowej (up) oraz poziomej (forward)
        // Mo�esz dostosowa� wag�, np. 0.2f oznacza, �e 20% si�y skoku idzie w kierunku przodu
        Vector3 jumpDirection = (Vector3.up).normalized;

        // Dodajemy impuls skoku
        rigidBody.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void MountPlayer(Transform mountPosition)
    {
        transform.position = mountPosition.position;
        mounted = true;
    }

    public void DeMount()
    {
        if (mounted)
        {
            Jump();
            mounted = false;
        }
    }
}
