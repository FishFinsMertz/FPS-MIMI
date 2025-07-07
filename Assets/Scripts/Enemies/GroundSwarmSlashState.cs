using System.Collections;
using UnityEngine;

public class GroundSwarmSlashState : GroundSwarmState
{
    public GroundSwarmSlashState(GroundSwarmController groundSwarm) : base(groundSwarm) { }

    public override void Enter()
    {
        groundSwarm.StartCoroutine(StartSlash());
    }

    private IEnumerator StartSlash()
    {
        yield return new WaitForSeconds(groundSwarm.slashChargeDuration);
        groundSwarm.slashHitBox.enabled = true;
        yield return new WaitForSeconds(groundSwarm.slashDuration);
        groundSwarm.slashHitBox.enabled = false;
        Debug.Log("ENEMY SLASHED PLAYER");

        groundSwarm.ChangeState(new GroundSwarmChaseState(groundSwarm));
    }
}
