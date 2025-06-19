using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GroundSwarmController : NetworkBehaviour
{
    [Header("CHASE STATS")]
    public float targetUpdateTime = 1f;
    public float moveSpeed;

    // Private or Hidden variables
    private GroundSwarmState currentState;
    private GameObject currentTarget;
    [HideInInspector] public List<GameObject> targets;
    [HideInInspector] public Rigidbody rb;
    private EventBinding<LocalPlayerSpawned> playerSpawnEventBinding;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ChangeState(new GroundSwarmChaseState(this));
        StartCoroutine(UpdateTargetRoutine());
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    void Update()
    {
        currentState.Update();
    }

    public void ChangeState(GroundSwarmState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    /*                                            FOR FUTURE: ADD A BETTER ALGORITHM THAT CHOOSES THE TARGET                                          */
    private IEnumerator UpdateTargetRoutine()
    {
        FindClosestTarget();
        //Debug.Log(currentTarget.transform.position);
        yield return new WaitForSeconds(targetUpdateTime);
    }
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    private void FindClosestTarget()
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

    // EVENTS
    private void OnEnable()
    {
        playerSpawnEventBinding = new EventBinding<LocalPlayerSpawned>(AddTarget);
        EventBus<LocalPlayerSpawned>.Register(playerSpawnEventBinding);
    }

    private void OnDisable()
    {
        EventBus<LocalPlayerSpawned>.Deregister(playerSpawnEventBinding);
    }

    private void AddTarget(LocalPlayerSpawned playerSpawnedEvent)
    {
        Debug.Log("New player joined");
        targets.Add(playerSpawnedEvent.playerGameObject.gameObject);
    }
}
