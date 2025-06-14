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
    private Vector2 lookInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Move").performed += OnMove;
        playerInput.actions.FindAction("Move").started += OnMove;
        playerInput.actions.FindAction("Move").canceled += OnMove;
        playerInput.actions.FindAction("Look").performed += OnLook;
        playerInput.actions.FindAction("Look").canceled += OnLook;
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
        // Debug.Log(playerInput.currentControlScheme);
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

    public void OnLook(InputAction.CallbackContext ctx) 
    {
        lookInput = ctx.ReadValue<Vector2>();
    }
    public Vector2 GetMoveInput() => moveInput;
    public Vector2 GetLookInput() => lookInput;
    public float GetMoveSpeed() => moveSpeed;

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            EventBus<LocalPlayerSpawned>.Raise(new LocalPlayerSpawned { playerController = this });
        }
    }
}
