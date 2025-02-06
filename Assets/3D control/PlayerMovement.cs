using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
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

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;

        readyToJump = true;

        // Debug: sprawdzenie, czy Rigidbody zostało poprawnie przypisane
        if (rigidBody == null)
        {
            //Debug.LogError("Rigidbody not found on the player object!");
        }
    }

    void Update()
    {
        // Sprawdzenie, czy gracz jest na ziemi
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 1.2f, groundCheckLayer);

        // Debug: informacje o stanie gracza
        //Debug.Log($"isGrounded: {isGrounded}, Velocity: {rigidBody.linearVelocity}");

        MyInput();

        if (isGrounded)
        {
            rigidBody.linearDamping = groundDrag;
            //Debug.Log("Player is grounded. Applying ground drag.");
        }
        else
        {
            rigidBody.linearDamping = 0f;
            //Debug.Log("Player is in the air. No drag applied.");
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        // Pobranie wejścia z klawiatury
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Debug: informacje o wejściu
        //Debug.Log($"HorizontalInput: {horizontalInput}, VerticalInput: {verticalInput}");

        // Sprawdzenie, czy gracz próbuje skoczyć
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;

            //Debug.Log("Player is jumping!");

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Debug: wyświetlenie kierunku ruchu
        //Debug.Log($"MoveDirection: {moveDirection}");

        if (isGrounded)
        {
            rigidBody.AddForce(moveDirection * moveSpeed, ForceMode.Force);
            //Debug.Log("Applying ground movement force.");
        }
        else if (!isGrounded)
        {
            rigidBody.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
            //Debug.Log("Applying air movement force.");
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);

        // Debug: wyświetlenie prędkości
        //Debug.Log($"Flat velocity: {flatVel.magnitude}");

        // Ograniczenie prędkości, jeśli potrzeba
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rigidBody.linearVelocity = new Vector3(limitedVel.x, rigidBody.linearVelocity.y, limitedVel.z);
            //Debug.Log("Velocity limited to moveSpeed.");
        }
    }

    private void Jump()
    {
        // Resetowanie prędkości pionowej
        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);

        rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        //Debug.Log("Jump force applied.");
    }

    private void ResetJump()
    {
        readyToJump = true;
        //Debug.Log("Player is ready to jump again.");
    }

    public void MountPlayer(Transform mountPosition)
    {
        transform.position = mountPosition.position;
        mounted = true;
        //Debug.Log("Player mounted at new position.");
    }

    public void DeMount()
    {
        if (mounted)
        {
            Jump();
            mounted = false;
            //Debug.Log("Player demounted.");
        }
    }
}
