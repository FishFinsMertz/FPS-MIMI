using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundSwarmState : BaseEnemyState
{
    protected GroundSwarmController groundSwarm => (GroundSwarmController)enemy;

    public GroundSwarmState(GroundSwarmController groundSwarm) : base(groundSwarm) { }

    public override void Enter() { base.Enter(); }
    public override void Update() { base.Update(); }
    public override void FixedUpdate() { base.FixedUpdate(); }
    public override void Exit() { base.Exit(); }
}

