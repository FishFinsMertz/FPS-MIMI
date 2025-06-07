using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private PlayerState currentState;


    void Start()
    {
        ChangeState(new PlayerWalkState(this)); //Temporary, make this idle
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

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }
}
