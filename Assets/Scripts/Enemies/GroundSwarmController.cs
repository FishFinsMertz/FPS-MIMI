using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundSwarmController : EnemyControllerBase
{
    [Header("Movement & Detection")]
    public float moveSpeed;
    public float slashRange;

    [Header("Attack Stats")]
    public float slashDuration;
    public float slashDmg;

    protected override void Start()
    {
        base.Start();
        ChangeState(new GroundSwarmChaseState(this));
    }

    protected override void OnDeath()
    {
        spawner.RecycleEnemy(this.gameObject);
    }
}
