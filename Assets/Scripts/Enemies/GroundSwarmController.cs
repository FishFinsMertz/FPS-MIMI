using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundSwarmController : EnemyControllerBase
{
    [Header("CHASE STATS")]
    public float targetUpdateTime = 1f;
    public float moveSpeed;

    // Private or Hidden variables
    private GroundSwarmState currentState;
    private GameObject currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ChangeState(new GroundSwarmChaseState(this));
        StartCoroutine(UpdateTargetRoutine());
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

    /*                                            FOR FUTURE: ADD A BETTER ALGORITHM THAT CHOOSES THE TARGET                                          */
    private IEnumerator UpdateTargetRoutine()
    {
        while (true)
        {
            FindClosestTarget();

            if (currentTarget != null)
                Debug.Log("Target: " + currentTarget.name + " at " + currentTarget.transform.position);

                yield return new WaitForSeconds(targetUpdateTime);
        }
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

    public override void OnDeath()
    {
        base.OnDeath();
    }
}
