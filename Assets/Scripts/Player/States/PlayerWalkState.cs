using UnityEngine;

public class PlayerWalkState : PlayerState
{
    private Rigidbody rb;

    public PlayerWalkState(PlayerController player) : base(player) 
    {
        rb = player.GetRigidbody();
    }

    public override void FixedUpdate()
    {
        Vector2 moveInput = player.GetMoveInput();
        Vector3 forwardDir = player.GetMoveDirection();
        
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        
        if (moveDir.magnitude > 0)
        {
            // align move direction by the camera's forward direction
            moveDir = Quaternion.LookRotation(forwardDir) * moveDir;
            moveDir.Normalize();
            
            // Apply movement using Rigidbody
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
        // Check for movement input to stay in walk state
        Vector2 moveInput = player.GetMoveInput();
        if (moveInput.magnitude == 0)
        {
            player.ChangeState(new PlayerIdleState(player));
        }
    }
    
    public override void OnJump()
    {
        // Transition to jump state when jump is pressed
        player.ChangeState(new PlayerJumpState(player, player.GetJumpForce()));
    }
}
