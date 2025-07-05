using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyState
{
    protected EnemyControllerBase enemy;

    List<Func<bool>> transitionGuards; 

    public BaseEnemyState(EnemyControllerBase enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
