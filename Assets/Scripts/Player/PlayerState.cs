using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController player;

    List<Func<bool>> transitionGuards; 

    public PlayerState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
