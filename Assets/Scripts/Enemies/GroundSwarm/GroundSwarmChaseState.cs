using UnityEngine;

public class GroundSwarmChaseState : GroundSwarmState
{
    public GroundSwarmChaseState(GroundSwarmController groundSwarm) : base(groundSwarm) { }

    public float rotationSpeed = 4f;
    public override void FixedUpdate()
    {
        groundSwarm.boidFlocking.enabled = true;
        groundSwarm.boidFlocking.target = groundSwarm.GetCurrentTarget().transform;
    }

    public override void Update()
    {
        if (groundSwarm.DistanceFromTarget(groundSwarm.GetCurrentTarget()) <= groundSwarm.slashRange)
        {
            groundSwarm.ChangeState(new GroundSwarmSlashState(groundSwarm));
        }
    }
    public override void Exit()
    {
        groundSwarm.boidFlocking.enabled = false;
    }
}
