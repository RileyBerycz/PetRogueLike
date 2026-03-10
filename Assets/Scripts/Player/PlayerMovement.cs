using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //This script handles basic player movement using the new Unity Input System
    //Move the player left or right at a variable speed with smooth acceleration and deceleration
    //Sprint when sprint input is held
    //Release space quickly for a normal jump
    //Hold space to charge - exponential curve, fires on release
    //Longer hold = exponentially higher jump up to maxChargeTime

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.8f;
    [Tooltip("Higher = snappier acceleration. 50 feels like a responsive animal.")]
    [SerializeField] private float acceleration = 50f;
    [Tooltip("Higher = stops faster. 40 feels natural without sliding.")]
    [SerializeField] private float deceleration = 40f;

    [Header("Jump")]
    [SerializeField] private float tapJumpForce = 24f;
    [SerializeField] private float minChargedJumpForce = 24f;
    [SerializeField] private float maxChargedJumpForce = 40f;
    [SerializeField] private float maxChargeTime = 5f;
    [SerializeField] private float jumpExponent = 2.5f;
    [Tooltip("How long space must be held before it counts as a charge instead of a tap")]
    [SerializeField] private float chargeThreshold = 0.3f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool showDebugUI = true;

    private Rigidbody2D rb;
    private bool isGrounded;

    // Jump state
    private bool isCharging;
    private float chargeTime;
    private bool jumpStartedGrounded;
    private bool jumpWasHeldLastFrame;

    // Movement state
    private Vector2 moveInput;
    private bool sprintHeld;
    private float currentSpeed;

    // Original scale for crouch restore
    private Vector3 originalScale;

    // Input action references
    private PlayerInput playerInput;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction moveAction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        moveAction = playerInput.actions["Move"];
    }

    void Update()
    {
        // Read all inputs directly every frame - most reliable approach
        sprintHeld = sprintAction.IsPressed();
        moveInput = moveAction.ReadValue<Vector2>();
        bool jumpIsHeld = jumpAction.IsPressed();

        // Check grounded
        isGrounded = Physics2D.OverlapCircle(
            transform.position - new Vector3(0, 0.75f, 0), 0.2f, groundLayer);

        // Detect the moment space is first pressed this frame
        bool jumpPressedThisFrame = jumpIsHeld && !jumpWasHeldLastFrame;

        // Detect the moment space is released this frame
        bool jumpReleasedThisFrame = !jumpIsHeld && jumpWasHeldLastFrame;

        // When jump is first pressed, record if we were grounded
        if (jumpPressedThisFrame && isGrounded)
        {
            jumpStartedGrounded = true;
            chargeTime = 0f;
            isCharging = false;
        }

        // While jump is held and started grounded, accumulate charge time
        if (jumpIsHeld && jumpStartedGrounded && isGrounded)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);

            // Only show crouch visual after charge threshold is passed
            if (chargeTime > chargeThreshold)
            {
                isCharging = true;

                float chargeProgress = (chargeTime - chargeThreshold) / (maxChargeTime - chargeThreshold);
                chargeProgress = Mathf.Clamp01(chargeProgress);

                float crouchAmount = Mathf.Lerp(1f, 0.5f, chargeProgress);
                transform.localScale = new Vector3(
                    originalScale.x, originalScale.y * crouchAmount, originalScale.z);
            }
        }

        // When jump is released, decide tap or charged jump
        if (jumpReleasedThisFrame && jumpStartedGrounded)
        {
            if (chargeTime <= chargeThreshold)
            {
                // Quick tap - normal hop
                FireTapJump();
            }
            else
            {
                // Held long enough - charged jump
                FireChargedJump();
            }

            // Reset jump state
            jumpStartedGrounded = false;
            isCharging = false;
            chargeTime = 0f;
            transform.localScale = originalScale;
        }

        // Cancel charge if player leaves ground while charging
        if (!isGrounded && isCharging)
        {
            isCharging = false;
            transform.localScale = originalScale;
        }

        // Store jump held state for next frame comparison
        jumpWasHeldLastFrame = jumpIsHeld;

        // Calculate target speed
        float targetSpeed;
        if (moveInput.x == 0)
        {
            targetSpeed = 0f;
        }
        else if (isCharging)
        {
            // Lock to walk speed while crouching
            targetSpeed = moveSpeed * Mathf.Abs(moveInput.x);
        }
        else
        {
            float speed = sprintHeld ? moveSpeed * sprintMultiplier : moveSpeed;
            targetSpeed = speed * Mathf.Abs(moveInput.x);
        }

        // Smoothly move toward target speed
        float rate = moveInput.x == 0 ? deceleration : acceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

        // Apply velocity preserving Y
        float direction = moveInput.x != 0
            ? Mathf.Sign(moveInput.x)
            : Mathf.Sign(rb.linearVelocity.x);
        rb.linearVelocity = new Vector2(currentSpeed * direction, rb.linearVelocity.y);
    }

    void FireTapJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, tapJumpForce);
    }

    void FireChargedJump()
    {
        float chargeProgress = (chargeTime - chargeThreshold) / (maxChargeTime - chargeThreshold);
        chargeProgress = Mathf.Clamp01(chargeProgress);

        float jumpForce = Mathf.Lerp(
            minChargedJumpForce,
            maxChargedJumpForce,
            Mathf.Pow(chargeProgress, jumpExponent));

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        transform.localScale = originalScale;
    }

    void OnGUI()
    {
        if (!showDebugUI) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        float chargeProgress = 0f;
        if (chargeTime > chargeThreshold)
        {
            chargeProgress = (chargeTime - chargeThreshold) / (maxChargeTime - chargeThreshold);
            chargeProgress = Mathf.Clamp01(chargeProgress);
        }

        GUI.Label(new Rect(10, 10, 400, 30), "isGrounded: " + isGrounded, style);
        GUI.Label(new Rect(10, 40, 400, 30), "isCharging: " + isCharging, style);
        GUI.Label(new Rect(10, 70, 400, 30), "chargeTime: " + chargeTime.ToString("F2") + "s", style);
        GUI.Label(new Rect(10, 100, 400, 30), "charge %: " + Mathf.Round(chargeProgress * 100f) + "%", style);
        GUI.Label(new Rect(10, 130, 400, 30), "sprintHeld: " + sprintHeld, style);
        GUI.Label(new Rect(10, 160, 400, 30), "currentSpeed: " + currentSpeed.ToString("F2"), style);
        GUI.Label(new Rect(10, 190, 400, 30), "velocity: " + rb.linearVelocity, style);
        GUI.Label(new Rect(10, 220, 400, 30), "jumpIsHeld: " + jumpAction.IsPressed(), style);
        GUI.Label(new Rect(10, 250, 400, 30), "jumpStartedGrounded: " + jumpStartedGrounded, style);
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position - new Vector3(0, 0.75f, 0), 0.2f);
    }
}