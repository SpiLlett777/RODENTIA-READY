using UnityEngine;

public class PersonMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;
    public float decceleration;
    public float velPower;
    [Space(10)]
    private float moveInput;
    [Space(10)]
    public float frictionAmount;

    [Header("Jump")]
    public float jumpForce;
    [Range(0, 1)]
    public float jumpCutMultiplier;
    [Space(10)]
    public float jumpCoyoteTime;
    [SerializeField] private float lastGroundedTime;
    public float jumpBufferTime;
    [SerializeField] private float lastJumpTime;
    [Space(10)]
    public float fallGravityMultiplier;
    private float gravityScale;
    [Space(10)]
    [SerializeField] private bool isJumping;
    
    [Header("Checks")]
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    [Space(10)]
    public LayerMask groundLayer;
    [Space(10)]
    
    [Header("Camera and Facing")]
    [SerializeField] private CameraFollowObject _cameraFollowGO;
    [SerializeField] private bool IsFacingRight;

    [SerializeField] private bool jumpInputReleased;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;

    private bool canMove = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        if (transform.rotation.y == 0)
        {
            IsFacingRight = true;
        }
        else if (transform.rotation.y == 180)
        {
            IsFacingRight = false;
        }
    }
    void Start()
    {
        gravityScale = rb.gravityScale;

        jumpInputReleased = true;

        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();

        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        if (!canMove)
        {
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", lastGroundedTime > 0);

        animator.SetFloat("VerticalSpeed", rb.velocityY);

        bool isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer) != null;

        animator.SetBool("IsJumping", !isGrounded);

        TurnCheck();

        #region Interpolation based on Vertical velocity
        if (rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        
        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
        #endregion

        #region Jump logic
        if (Input.GetKey(KeyCode.C))
        {
            lastJumpTime = jumpBufferTime;
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            OnJumpUp();
        }

        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            lastGroundedTime = jumpCoyoteTime;
        }

        if (rb.velocity.y < 0)
        {
            isJumping = false;
        }

        if (lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping && jumpInputReleased)
        {
            Jump();
        }
        #endregion

        #region Changing gravity when you start falling
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        #endregion

        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        Run();
        Friction();
    }
    public void Run()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }
    public void Friction()
    {
        if (lastGroundedTime > 0 && rb.velocity.magnitude < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastGroundedTime = 0;
        lastJumpTime = 0;
        isJumping = true;
        jumpInputReleased = false;
    }
    public void OnJump()
    {
        lastJumpTime = jumpBufferTime;
    }
    public void OnJumpUp()
    {
        if (rb.velocity.y > 0 && isJumping)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }
        jumpInputReleased = true;
        lastJumpTime = 0;
    }
    private void TurnCheck()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0 && !IsFacingRight)
        {
            Turn();
        }
        else if (moveInput < 0 && IsFacingRight)
        {
            Turn();
        }
    }
    private void Turn()
    {
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(0f, -180f, 0f);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;

            _cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(0f, 0f, 0f);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;

            _cameraFollowObject.CallTurn();
        }
    }
    public void DisableMovement()
    {
        moveInput = 0f;
        rb.velocity = new Vector2(0f, 0f);
        canMove = false;
    }
    public void EnableMovement()
    {
        canMove = true;
    }
}