using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;

public abstract class EnemyControllerBase : NetworkBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public List<GameObject> targets;
    [HideInInspector] public EnemySpawner spawner;
    
    // Events
    private EventBinding<PlayerSpawnedEvent> playerSpawnEventBinding;
    private EventBinding<PlayerLeftEvent> playerLeftEventBinding;

    // Other Hidden Variables
    private float targetUpdateTime = 1f;
    protected GroundSwarmState currentState;
    protected GameObject currentTarget;

    protected virtual void Start()
    {
        StartCoroutine(UpdateTargetRoutine());
    }

    // EVENTS
    private void OnEnable()
    {
        //Debug.Log("Binding event");

        // Register events
        playerSpawnEventBinding = new EventBinding<PlayerSpawnedEvent>(AddTarget);
        EventBus<PlayerSpawnedEvent>.Register(playerSpawnEventBinding);

        playerLeftEventBinding = new EventBinding<PlayerLeftEvent>(RemoveTarget);
        EventBus<PlayerLeftEvent>.Register(playerLeftEventBinding);

        // FOR DEATH LOGIC WITH HEALTH COMPONENT
        if (TryGetComponent<HealthComponent>(out var health))
        {
            health.onDeath.AddListener(OnDeath);
        }
    }

    private void OnDisable()
    {
        // Deregister events
        EventBus<PlayerSpawnedEvent>.Deregister(playerSpawnEventBinding);
        EventBus<PlayerLeftEvent>.Deregister(playerLeftEventBinding);

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
        /*foreach (GameObject target in targets)
        {
            Debug.Log(target.name);
        }
        */
    }

    public void RemoveTarget(PlayerLeftEvent playerLeftEvent)
    {
        if (targets.Contains(playerLeftEvent.playerGameObject.gameObject))
        {
            targets.Remove(playerLeftEvent.playerGameObject.gameObject);
            Debug.Log("Removed target: " + playerLeftEvent.playerGameObject.gameObject.name);
        }
    }

        /*                                            FOR FUTURE: ADD A BETTER ALGORITHM THAT CHOOSES THE TARGET                                          */
    public IEnumerator UpdateTargetRoutine()
    {
        while (true)
        {
            FindClosestTarget();

            if (currentTarget != null)
            {
                //Debug.Log("Target: " + currentTarget.name + " at " + currentTarget.transform.position);
            }
            yield return new WaitForSeconds(targetUpdateTime);
        }
    }

    public virtual GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    protected virtual void FindClosestTarget()
    {
        float temp = Mathf.Infinity;
        foreach (GameObject target in targets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= temp)
            {
                currentTarget = target;
                temp = distanceToTarget;
            }
        }
    }

    public virtual void OnDeath()
    {
        // Default behavior: disable the object
        Debug.Log("Rip Bozo");
    }
}
