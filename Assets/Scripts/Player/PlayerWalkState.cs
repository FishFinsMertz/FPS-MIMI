using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerController player) : base(player) { }

    public override void FixedUpdate()
    {
        // Make dis shit better brah, use unity velocity

        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            moveDir.x += 1;
        }
        player.transform.position += moveDir * Time.deltaTime * 5;
    }

    public override void Update()
    {
        //Change states here with conditionals for now
        return;
    }
}
