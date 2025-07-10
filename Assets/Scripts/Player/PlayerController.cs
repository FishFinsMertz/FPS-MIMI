using System;
using System.ComponentModel;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask groundLayer = 1; // Default layer
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jetpackActivationDelay = 0.3f;
    
    [SerializeField, ReadOnly(true)] private PlayerState currentState;
    public event Action<string> OnStateChanged;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 moveDirection;

    // -- object references
    private Transform cameraTransform;
    private Transform feetPosition;
    private Rigidbody rb;
    public LocalEventBusManager localEventBusManager  { get; private set; } = new LocalEventBusManager();
    [SerializeReference] List<MonoBehaviour> components = new List<MonoBehaviour>();

    public override void OnNetworkSpawn()
    {
        // Raise events
        if (IsLocalPlayer)
        {
            EventBus<LocalPlayerSpawned>.Raise(new LocalPlayerSpawned
            {
                playerController = this,
                playerGameObject = this.gameObject
            });
            foreach (var component in components)
            {
                if (component is IComponent iComponent)
                {
                    iComponent.Initialize(localEventBusManager);
                }
            }
        }

        EventBus<PlayerSpawnedEvent>.Raise(new PlayerSpawnedEvent
        {
            playerController = this,
            playerGameObject = this.gameObject
        });

        // Input
        if (!IsOwner) return;
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Move").performed += OnMove;
        playerInput.actions.FindAction("Move").started += OnMove;
        playerInput.actions.FindAction("Move").canceled += OnMove;
        playerInput.actions.FindAction("Look").performed += OnLook;
        playerInput.actions.FindAction("Look").canceled += OnLook;
        playerInput.actions.FindAction("Attack").performed += OnAttack;
        // playerInput.actions.FindAction("Jump").started += OnJetpackStart;
        // playerInput.actions.FindAction("Jump").canceled += OnJetpackEnd;
        playerInput.actions.FindAction("Jump").performed += OnJump;
        playerInput.actions.FindAction("Jump").canceled += OnStopJump;
    }

    public override void OnNetworkDespawn()
    {
        EventBus<PlayerLeftEvent>.Raise(new PlayerLeftEvent
        {
            playerController = this,
            playerGameObject = this.gameObject
        });
    }

    void Awake()
    {
        cameraTransform = Camera.main.transform;
        feetPosition = transform.Find("PlayerFeet");
        rb = GetComponent<Rigidbody>();
        
        // Add Rigidbody if it doesn't exist
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true; // Prevent rotation from physics
            rb.angularVelocity = Vector3.zero;
        }
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
        //Debug.Log(currentState);  
        currentState.Update(); 
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
        OnStateChanged?.Invoke(currentState.GetType().Name);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        // Delegate to current state
        currentState?.OnMove(moveInput);
    }

    public void OnLook(InputAction.CallbackContext ctx) 
    {
        lookInput = ctx.ReadValue<Vector2>();
        moveDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        // Delegate to current state
        currentState?.OnLook(lookInput);
    }
    
    public void OnAttack(InputAction.CallbackContext ctx) 
    {
        localEventBusManager.GetLocalEventBus<ShootEvent>().Raise(new ShootEvent{
            bulletOrigin = Camera.main.transform.position,
            targetDirection = Camera.main.transform.forward,
        }, true);
        // Delegate to current state
        currentState?.OnAttack();
    }
    
    public void OnJump(InputAction.CallbackContext ctx) 
    {
        // Delegate to current state
        currentState?.OnJump();
    }

    public void OnStopJump(InputAction.CallbackContext ctx) 
    {
        // Delegate to current state
        currentState?.OnStopJump();
    }

    public Vector2 GetMoveInput() => moveInput;
    public Vector2 GetLookInput() => lookInput;
    public Vector3 GetMoveDirection() => moveDirection;
    public float GetMoveSpeed() => moveSpeed;
    public float GetJumpForce() => jumpForce;
    public float GetJetpackActivationDelay() => jetpackActivationDelay;
    public Rigidbody GetRigidbody() => rb;
    public LayerMask GetGroundLayer() => groundLayer;
    public Vector3 GetFeetPosition() => feetPosition.position;
}
