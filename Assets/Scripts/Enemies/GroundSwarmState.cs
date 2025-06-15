using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundSwarmState
{
    protected GroundSwarmController groundSwarm;

    List<Func<bool>> transitionGuards; 

    public GroundSwarmState(GroundSwarmController groundSwarm)
    {
        this.groundSwarm = groundSwarm;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
