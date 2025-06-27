using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public abstract class EnemyControllerBase : NetworkBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public List<GameObject> targets;
    [HideInInspector] public EnemySpawner spawner;
    
    private EventBinding<PlayerSpawnedEvent> playerSpawnEventBinding;
    
        // EVENTS
    private void OnEnable()
    {
        Debug.Log("Binding event");
        playerSpawnEventBinding = new EventBinding<PlayerSpawnedEvent>(AddTarget);
        EventBus<PlayerSpawnedEvent>.Register(playerSpawnEventBinding);

        // FOR DEATH LOGIC WITH HEALTH COMPONENT
        if (TryGetComponent<HealthComponent>(out var health))
        {
            health.onDeath.AddListener(OnDeath);
        }
    }

    private void OnDisable()
    {
        EventBus<PlayerSpawnedEvent>.Deregister(playerSpawnEventBinding);

        // FOR DEATH LOGIC WITH HEALTH COMPONENT
        if (TryGetComponent<HealthComponent>(out var health))
        {
            health.onDeath.RemoveListener(OnDeath);
        }
    }

    private void AddTarget(PlayerSpawnedEvent playerSpawnedEvent)
    {
        Debug.Log("player added to targets list" + playerSpawnedEvent.playerGameObject.gameObject.name);
        targets.Add(playerSpawnedEvent.playerGameObject.gameObject);
        foreach (GameObject target in targets)
        {
            Debug.Log(target.name);
        }
    }

    public virtual void OnDeath()
    {
        // Default behavior: disable the object
        Debug.Log("Rip Bozo");
    }
}
