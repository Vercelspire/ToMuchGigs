using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 7f;
    public float groundDrag = 5f;
    public float jumpForce = 7f;
    public float jumpCooldown = 0.5f;
    public float airMultiplier = 1.5f;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed = 3f;
    public float crouchYScale = 0.5f;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight = 2f; 
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    [Header("Sprint Settings")]
    public float sprintDuration = 2f;
    public float sprintCooldown = 6f;
    public float sprintMultiplier = 1.5f;

    private bool isSprinting = false;
    private bool readyToSprint = true;
    private float cooldownTimer = 0f;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public bool isCrouching;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        StateHandler();
        SpeedControl();
        HandleSprintCooldown();

        rb.drag = grounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        if (Input.GetKey(sprintKey) && readyToSprint && grounded && !isSprinting)
        {
            StartCoroutine(Sprint());
        }

        if (Input.GetKeyUp(sprintKey) && isSprinting)
        {
            StopSprint();
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
        }
        else if (isSprinting)
        {
            state = MovementState.sprinting;
        }
        else if (grounded)
        {
            state = MovementState.walking;
        }
        else
        {
            state = MovementState.air;
        }
        isCrouching = (state == MovementState.crouching);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        float currentSpeed = walkSpeed;

        if (state == MovementState.crouching) currentSpeed = crouchSpeed;
        else if (state == MovementState.sprinting) currentSpeed = sprintSpeed * sprintMultiplier;

        if (grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float maxSpeed = walkSpeed;

        if (state == MovementState.crouching) maxSpeed = crouchSpeed;
        else if (state == MovementState.sprinting) maxSpeed = sprintSpeed * sprintMultiplier;

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        grounded = false; // ADDED
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private IEnumerator Sprint()
    {
        isSprinting = true;
        readyToSprint = false;

        float timer = 0f;
        while (timer < sprintDuration && Input.GetKey(sprintKey))
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StopSprint();
    }

    private void StopSprint()
    {
        if (isSprinting)
        {
            isSprinting = false;
            cooldownTimer = sprintCooldown;
        }
    }

    private void HandleSprintCooldown()
    {
        if (!readyToSprint)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) readyToSprint = true;
        }
    }
}
