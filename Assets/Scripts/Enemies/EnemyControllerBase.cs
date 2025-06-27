using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public abstract class EnemyControllerBase : NetworkBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public List<GameObject> targets;

    [HideInInspector] public EnemySpawner spawner;
    
    public virtual void OnDeath()
    {
        // Default behavior: disable the object
        gameObject.SetActive(false);
    }
}
