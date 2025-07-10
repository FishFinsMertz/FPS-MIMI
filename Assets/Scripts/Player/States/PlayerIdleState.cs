using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player) : base(player) { }

    public override void Update()
    {
        // Check for movement input to transition to walk state
        Vector2 moveInput = player.GetMoveInput();
        if (moveInput.magnitude > 0)
        {
            player.ChangeState(new PlayerWalkState(player));
        }
    }
    
    public override void OnJump()
    {
        // Transition to jump state when jump is pressed
        player.ChangeState(new PlayerJumpState(player, 8f));
    }
}
