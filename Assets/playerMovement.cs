
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float slidingSpeed = 7f;

    [Header("Jump and Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -19.62f;

    [Header("Crouching")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;

    private CharacterController controller;
    private Vector3 velocity;

    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private bool isSliding;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Set the initial standing height.
        controller.height = standingHeight;
        controller.center = new Vector3(0f, standingHeight / 2f, 0f);
    }

    void Update()
    {
        // Check whether the player is grounded.
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        // Get movement input.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // Toggle crouching once per key press.
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                controller.height = crouchingHeight;
                controller.center = new Vector3(0f, crouchingHeight / 2f, 0f);
            }
            else
            {
                controller.height = standingHeight;
                controller.center = new Vector3(0f, standingHeight / 2f, 0f);
            }
        }
        // Determine sprinting and sliding states.
        isSprinting = Input.GetKey(KeyCode.LeftShift)
                      && !isCrouching;
        isSliding = isCrouching
                    && Input.GetKey(KeyCode.LeftShift)
                    && isGrounded;
        // Select the movement speed BEFORE moving.
        float currentSpeed;
        if (isSliding)
        {
            currentSpeed = slidingSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
        // Move relative to the player's facing direction.
        Vector3 move = transform.right * x + transform.forward * z;
        move = Vector3.ClampMagnitude(move, 1f);
        controller.Move(move * currentSpeed * Time.deltaTime);
        // Jump only when grounded and not crouching/sliding.
        if (Input.GetButtonDown("Jump")
            && isGrounded
            && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        // Apply gravity.
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
