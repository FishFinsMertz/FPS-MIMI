using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundSwarmController : EnemyControllerBase
{
    [Header("Detection")]
    public float slashRange;
    public BoidFlockingComponent boidFlocking;

    [Header("Attack Hitboxes")]
    public Collider slashHitBox;

    [Header("Attack Stats")]
    public float slashChargeDuration;
    public float slashDuration;

    protected override void Start()
    {
        base.Start();
        ChangeState(new GroundSwarmChaseState(this));
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // Temporary to turn off all hitboxes
        slashHitBox.enabled = false;

        // Restart state machine
        if (!IsServer) return;
         ChangeState(new GroundSwarmChaseState((GroundSwarmController)this));
    }

    protected override void OnDeath()
    {
        spawner.RecycleEnemy(this.gameObject);
    }
}
