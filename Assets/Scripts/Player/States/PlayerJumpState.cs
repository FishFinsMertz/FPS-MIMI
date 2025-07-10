using UnityEngine;

public class PlayerJumpState : PlayerState
{
    [Header("Jump Settings")]
    private float jumpForce;
    private bool isGrounded = true;
    private float groundCheckDistance = 0.1f;
    private Rigidbody rb;
    
    [Header("Jetpack Activation")]
    private float jetpackActivationDelay; // Delay before jetpack can be activated
    private float jumpStartTime;
    private bool canActivateJetpack = false;

    public PlayerJumpState(PlayerController player, float jumpForce) : base(player) 
    {
        rb = player.GetRigidbody();
        this.jumpForce = jumpForce;
        this.jetpackActivationDelay = player.GetJetpackActivationDelay();
    }

    public override void Enter()
    {
        // Apply jump force using Rigidbody
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        jumpStartTime = Time.time;
        canActivateJetpack = false;
        Debug.Log("Jumping");
    }

    public override void FixedUpdate()
    {
        isGrounded = Physics.Raycast(player.GetFeetPosition(), Vector3.down, groundCheckDistance);

        // Handle horizontal movement using Rigidbody
        Vector2 moveInput = player.GetMoveInput();
        Vector3 forwardDir = player.GetMoveDirection();
        
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        
        if (moveDir.magnitude > 0)
        {
            // align move direction by the camera's forward direction
            moveDir = Quaternion.LookRotation(forwardDir) * moveDir;
            moveDir.Normalize();
            
            // Apply horizontal movement using Rigidbody
            Vector3 horizontalVelocity = moveDir * player.GetMoveSpeed();
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }
        else
        {
            // Stop horizontal movement but preserve vertical velocity
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public override void Update()
    {
        // Transition back to walk state when grounded and falling
        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            Debug.Log("Grounded");
            player.ChangeState(new PlayerWalkState(player));
        }
    }

    public override void Exit()
    {
        // No need to reset velocity - Rigidbody handles this
    }
    
    public override void OnJump()
    {
        if (canActivateJetpack || Time.time - jumpStartTime >= jetpackActivationDelay){
            canActivateJetpack = true;
            player.ChangeState(new PlayerJetpackState(player));
        }
    }
}
