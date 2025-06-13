using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerController player) : base(player) { }

    public override void FixedUpdate()
    {
        Vector2 moveInput = player.GetMoveInput();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        
        if (moveDir.magnitude > 0)
        {
            moveDir.Normalize();
            player.transform.position += moveDir * Time.deltaTime * player.GetMoveSpeed();
        }
    }

    public override void Update()
    {
        // Change states here with conditionals for now
        return;
    }
}
