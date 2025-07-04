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
        rb = GetComponent<Rigidbody>();
        ChangeState(new GroundSwarmChaseState(this));
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        currentState.FixedUpdate();
    }

    void Update()
    {
        if (!IsServer) return;
        currentState.Update();

        //Debug.Log(spawner);
    }

    public void ChangeState(GroundSwarmState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }
}
