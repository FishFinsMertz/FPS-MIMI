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
        yield return new WaitForSeconds(groundSwarm.slashDuration);
        Debug.Log("ENEMY SLASHED PLAYER FOR " + groundSwarm.slashDmg + " DAMAGE");
        groundSwarm.ChangeState(new GroundSwarmChaseState(groundSwarm));
    }
}
