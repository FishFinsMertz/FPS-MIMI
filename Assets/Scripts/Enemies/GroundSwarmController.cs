using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundSwarmController : EnemyControllerBase
{
    [Header("CHASE STATS")]
    public float moveSpeed;

    protected override void Start()
    {
        base.Start();
        ChangeState(new GroundSwarmChaseState(this));
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }
}
