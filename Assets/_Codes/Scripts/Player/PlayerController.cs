using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float swingSpeed;
    [SerializeField] float groundDrag;
    float moveSpeed;
    [HideInInspector] public bool isSwinging;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode escMenuKey = KeyCode.Escape;

    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;

    [Header("Ground Check")]
    [SerializeField] Transform GroundChecker;
    [SerializeField] float playerHeight;
    [SerializeField] string[] groundLayers = new string[] { "Ground", "Grab", "Grapple" };
    LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Footsteps SFX")]
    [SerializeField] AudioClip[] footSteps;
    AudioSource audioSource;

    [Header("Configs")]
    [SerializeField] Transform orientation;
    [SerializeField] float gravityValue;


    // key inputs
    float horizontalInput;
    float verticalInput;

    (Vector3, Quaternion) initialPosition;
    Vector3 moveDirection;
    Rigidbody rb;
    bool isPaused;
    bool isPlaying;
    Vector3 flatVel;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
        whatIsGround = LayerMask.GetMask(groundLayers);
        readyToJump = true;
        initialPosition = (transform.position, transform.rotation);
    }

    private void Update()
    {
        // ground check
        grounded = Physics.CheckSphere(GroundChecker.position, .3f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        PlayerBounds();

        if (grounded && verticalInput != 0 || horizontalInput != 0 && rb.velocity.y! > 1)
            PlayFootstepSound();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate() => MovePlayer();

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //esc menu
        if (Input.GetKeyDown(escMenuKey))
        {
            isPaused = !isPaused;
            if (isPaused)
                LevelSceneManager.Instance.PauseGame(pauseMenu);
            else
                LevelSceneManager.Instance.ResumeGame(pauseMenu);
        }
    }

    private void StateHandler()
    {
        // state - Sprinting
        if (grounded && Input.GetKey(sprintKey))
            moveSpeed = sprintSpeed;

        // state - Walking
        else if (grounded)
            moveSpeed = walkSpeed;

        // state - swinging/grapple
        else if (isSwinging)
            moveSpeed = swingSpeed;
    }

    private void MovePlayer()
    {
        if (isSwinging) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);
            rb.useGravity = false;
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);

        // in air
        else if (!grounded && !isSwinging)
        {
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);
            rb.AddForce(gravityValue * rb.mass * Physics.gravity, ForceMode.Force);
        }

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * .8f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void PlayerBounds()
    {
        // if the player somehow glitches out the map, respawn's the player at default spawn
        if (transform.position.y < -40)
            TeleportPlayer();
    }

    void TeleportPlayer()
    {
        var (pos, rot) = initialPosition;
        transform.SetPositionAndRotation(pos, rot);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Physics.SyncTransforms();
    }

    public void TransitionLevel(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Physics.SyncTransforms();
    }

    public void PlayFootstepSound()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            int index = Random.Range(0, footSteps.Length);
            AudioClip footstepSound = footSteps[index];
            audioSource.PlayOneShot(footstepSound);
            audioSource.pitch = flatVel.magnitude > 10f ? 2.5f : 1f;
            StartCoroutine(Cooldown(footstepSound.length));
        }
    }

    IEnumerator Cooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPlaying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            Transform checkpoint = other.gameObject.transform;
            initialPosition = (checkpoint.position, checkpoint.rotation);
        }

        if (other.gameObject.CompareTag("Finish"))
            LevelSceneManager.Instance.LevelFinish();

        if (other.gameObject.CompareTag("Deadzone"))
            TeleportPlayer();

        if (other.gameObject.CompareTag("Teleport"))
        {
            for (int i = 0; i < other.transform.childCount; i++)
            {
                Transform childTransform = other.transform.GetChild(i);

                if (childTransform.name == "Destination")
                {
                    transform.SetPositionAndRotation(childTransform.position, childTransform.rotation);
                    Physics.SyncTransforms();
                }
            }
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("RotatingObject"))
        {
            //Get the contact point and normals
            ContactPoint contact = other.contacts[0];
            Vector3 normal = contact.normal;

            //Calculate the reflection vector
            Vector3 reflection = Vector3.Reflect(orientation.transform.forward, normal);

            //Apply the reflection force to the colliding object
            rb.AddForce(reflection * 10f, ForceMode.Impulse);
        }
    }
}
