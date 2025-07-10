using UnityEngine;

public class PlayerJetpackState : PlayerState
{
    [Header("Jetpack Settings")]
    [SerializeField] private float jetpackForce = 15f;
    [SerializeField] private float maxJetpackTime = 3f;
    private float jetpackStartTime;
    private Rigidbody rb;

    public PlayerJetpackState(PlayerController player) : base(player) 
    {
        rb = player.GetRigidbody();
    }

    public override void Enter()
    {
        jetpackStartTime = Time.time;
        Debug.Log("Jetpack activated");
    }

    public override void FixedUpdate()
    {
        // Apply upward jetpack force
        rb.AddForce(Vector3.up * jetpackForce, ForceMode.Force);
        
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
        // // Check if jetpack time has expired
        // if (Time.time - jetpackStartTime >= maxJetpackTime)
        // {
        //     Debug.Log("Jetpack time expired");
        //     player.ChangeState(new PlayerWalkState(player));
        // }
    }

    public override void OnStopJump()
    {
        Debug.Log("Jetpack deactivated");
        player.ChangeState(new PlayerJumpState(player, 0f));
    }
}