using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    private PlayerState currentState;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Move").performed += OnMove;
        playerInput.actions.FindAction("Move").started += OnMove;
        playerInput.actions.FindAction("Move").canceled += OnMove;
    }

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
        Debug.Log(playerInput.currentControlScheme);
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

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public Vector2 GetMoveInput() => moveInput;
    public float GetMoveSpeed() => moveSpeed;
}
