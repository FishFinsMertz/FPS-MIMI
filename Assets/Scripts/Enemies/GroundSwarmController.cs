using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class GroundSwarmController : NetworkBehaviour
{
    [Header("CHASE TARGETS")] // DELETE THIS ONCE A BETTER SYSTEM IS MADE WITH NETWORKING IN MIND
    public List<GameObject> targets;

    [Header("CHASE STATS")]
    public float targetUpdateTime = 1f;
    public float moveSpeed;

    // Private or Hidden variables
    private GroundSwarmState currentState;
    private GameObject currentTarget;
    [HideInInspector] public Rigidbody rb;

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
        //Debug.Log(currentState);
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
        while (true)
        {
            FindClosestTarget();
            yield return new WaitForSeconds(targetUpdateTime);
            Debug.Log(GetCurrentTarget());
        }
    }
    public GameObject GetCurrentTarget()
    {
        FindClosestTarget();
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
}
