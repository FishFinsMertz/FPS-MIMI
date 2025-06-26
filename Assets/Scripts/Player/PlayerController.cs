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

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            EventBus<LocalPlayerSpawned>.Raise(new LocalPlayerSpawned { playerController = this });
        }
        if (!IsOwner) return;
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Move").performed += OnMove;
        playerInput.actions.FindAction("Move").started += OnMove;
        playerInput.actions.FindAction("Move").canceled += OnMove;
        playerInput.actions.FindAction("Look").performed += OnLook;
        playerInput.actions.FindAction("Look").canceled += OnLook;
        playerInput.actions.FindAction("Attack").performed += OnAttack;
        playerInput.actions.FindAction("Jump").started += OnJetpackStart;
        playerInput.actions.FindAction("Jump").canceled += OnJetpackEnd;
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
    public void OnAttack(InputAction.CallbackContext ctx) 
    {
        EventBus<ShootEvent>.Raise(new ShootEvent{
            gunOwner = gameObject,
            bulletOrigin = Camera.main.transform.position,
            targetDirection = Camera.main.transform.forward,
        });
    }
    public void OnJetpackStart(InputAction.CallbackContext ctx) 
    {
        EventBus<JetpackStart>.Raise(new JetpackStart { jetpackOwner = gameObject });
    }
    public void OnJetpackEnd(InputAction.CallbackContext ctx)
    {
        EventBus<JetpackEnd>.Raise(new JetpackEnd { jetpackOwner = gameObject });
    }
    public Vector2 GetMoveInput() => moveInput;
    public Vector2 GetLookInput() => lookInput;
    public float GetMoveSpeed() => moveSpeed;
}
