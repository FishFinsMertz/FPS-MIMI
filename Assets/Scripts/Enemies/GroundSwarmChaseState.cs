using UnityEngine;

public class GroundSwarmChaseState : GroundSwarmState
{
    public GroundSwarmChaseState(GroundSwarmController groundSwarm) : base(groundSwarm) { }

    public override void FixedUpdate()
    {
        GameObject target = groundSwarm.GetCurrentTarget();
        if (target == null) return;

        Vector3 direction = (target.transform.position - groundSwarm.transform.position).normalized;
        groundSwarm.rb.MovePosition(groundSwarm.transform.position + direction * groundSwarm.moveSpeed * Time.fixedDeltaTime);
    }
}
