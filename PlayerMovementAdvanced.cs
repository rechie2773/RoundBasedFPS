using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching/Sliding")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    [HideInInspector]
    public bool slide = false;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    // Audio
    [SerializeField]
    private AudioSource footStepAudio;
    [SerializeField]
    private AudioSource slideAudio;
    [SerializeField]
    private AudioSource sprintAudio;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;



    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
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
        if (GameOverManager.isGameOver) return;
        //check if audio still running
        if (Time.timeScale == 0f)
        {
            if (footStepAudio.isPlaying)
                footStepAudio.Pause();
            if (slideAudio.isPlaying)
                slideAudio.Pause();
            return;
        }
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Drag handling
        rb.drag = grounded ? groundDrag : 0;

        // Manage audio
        HandleAudio();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Start crouch/slide
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            if (grounded && rb.velocity.magnitude > 0.1f)
            {
                slide = true; // Activate slide
                desiredMoveSpeed = slideSpeed;
            }
        }

        // Stop crouch/slide
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            slide = false; // Deactivate slide
        }
    }

    private void StateHandler()
    {
        // Sliding
        if (slide)
        {
            state = MovementState.sliding;
        }
        // Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        // Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        // Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // Air
        else
        {
            state = MovementState.air;
            desiredMoveSpeed = walkSpeed;
        }

        // Update movement speed smoothly
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (OnSlope() && rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void HandleAudio()
    {
       
        if (state == MovementState.walking && rb.velocity.magnitude > 0.1f)
        {
            if (!footStepAudio.isPlaying) footStepAudio.Play();
            if (sprintAudio.isPlaying) sprintAudio.Stop();
            if (slideAudio.isPlaying) slideAudio.Stop();
        }
        else if (state == MovementState.sprinting && rb.velocity.magnitude > 0.1f)
        {
            if (footStepAudio.isPlaying) footStepAudio.Stop(); 
            if (!sprintAudio.isPlaying) sprintAudio.Play();
            if (slideAudio.isPlaying) slideAudio.Stop();
        }
        else if (state == MovementState.sliding)
        {
            if (footStepAudio.isPlaying) footStepAudio.Pause();
            if (sprintAudio.isPlaying) sprintAudio.Stop();
            if (!slideAudio.isPlaying) slideAudio.Play();
        }
        else
        {
            if (footStepAudio.isPlaying) footStepAudio.Pause();
            if (sprintAudio.isPlaying) sprintAudio.Stop();
            if (slideAudio.isPlaying) slideAudio.Stop();
        }
    }
    public void PauseAudio()
    {
        if (footStepAudio != null && footStepAudio.isPlaying)
        {
            footStepAudio.Pause();
        }

        if (slideAudio != null && slideAudio.isPlaying)
        {
            slideAudio.Pause();
        }

        if (sprintAudio != null && sprintAudio.isPlaying)
        {
            sprintAudio.Pause();
        }
    }

    public void ResumeAudio()
    {
        if (footStepAudio != null && !footStepAudio.isPlaying)
        {
            footStepAudio.UnPause();
        }

        if (slideAudio != null && !slideAudio.isPlaying)
        {
            slideAudio.UnPause();
        }
        if (sprintAudio != null && !sprintAudio.isPlaying)
        {
            sprintAudio.UnPause();
        }
    }
}