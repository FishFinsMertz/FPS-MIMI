using System.Collections;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Game Design Specs")]
    [SerializeField] private float cameraBaseMultiplier = 20;
    [SerializeField] private Vector3 cameraPlayerOffset = new Vector3(0,1,0);
    [SerializeField] private float cameraXMinRotation = -90;
    [SerializeField] private float cameraXMaxRotation = 90;
    [SerializeField] private float recoilTime = 0.1f; //These values should probably be attributes of the gun component
    [SerializeField] private float recoilReturnTimeMultiplier = 0.08f; //These values should probably be attributes of the gun component
    [SerializeField] private float recoilStrength = 3f;

    [Header("Player Specs")]
    [SerializeField] private float cameraSensitivity = 1;

    private Camera myCamera;
    private PlayerController playerController;
    private Vector3 lookDirection;

    private Coroutine recoilCoroutine = null;

    private Vector3 lookOffsetInitial = Vector3.zero;
    private Vector3 lookOffsetCurrent = Vector3.zero;
    private Vector3 lookOffsetTarget = Vector3.zero;
    private float lookOffsetTimer = 0;

    private EventBinding<LocalPlayerSpawned> playerSpawnEventBinding;
    private EventBinding<ShootAfterFXEvent> shootAfterFXEventBinding;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCamera = Camera.main;
    }

    private void OnEnable()
    {
        playerSpawnEventBinding = new EventBinding<LocalPlayerSpawned>(Initialize);
        EventBus<LocalPlayerSpawned>.Register(playerSpawnEventBinding);

        shootAfterFXEventBinding = new EventBinding<ShootAfterFXEvent>(Recoil);
        EventBus<ShootAfterFXEvent>.Register(shootAfterFXEventBinding);
    }

    private void OnDisable()
    {
        EventBus<LocalPlayerSpawned>.Deregister(playerSpawnEventBinding);
        EventBus<ShootAfterFXEvent>.Deregister(shootAfterFXEventBinding);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController) { return; }
        Vector2 lookDir = playerController.GetLookInput();
        lookDirection.x -= lookDir.y * Time.deltaTime * cameraBaseMultiplier * cameraSensitivity;
        lookDirection.x = Mathf.Clamp(lookDirection.x, cameraXMinRotation, cameraXMaxRotation);
        lookDirection.y += lookDir.x * Time.deltaTime * cameraBaseMultiplier * cameraSensitivity;

        Vector3 camDir = lookDirection + lookOffsetCurrent;
        camDir.x = Mathf.Clamp(camDir.x, cameraXMinRotation, cameraXMaxRotation);
        myCamera.transform.eulerAngles = camDir;
        
        myCamera.transform.position = playerController.transform.position + cameraPlayerOffset;
    }

    void Initialize(LocalPlayerSpawned playerSpawnedEvent) 
    {
        playerController = playerSpawnedEvent.playerController;
        Cursor.lockState = CursorLockMode.Locked;
        lookDirection = myCamera.transform.eulerAngles;
        lookOffsetCurrent = Vector3.zero;
        lookOffsetTimer = 0;
    }

    void Recoil(ShootAfterFXEvent shootAfterFXEvent) 
    {
        if (shootAfterFXEvent.gunOwner != playerController.gameObject) return;
        Debug.Log("Should Be Shooting");

        lookOffsetInitial = lookOffsetCurrent;
        lookOffsetTarget = lookOffsetInitial + Vector3.left * recoilStrength;
        lookOffsetTimer = 0;
        if (recoilCoroutine == null) recoilCoroutine = StartCoroutine(RecoilCoroutine());
    }

    IEnumerator RecoilCoroutine() 
    {
        float returnTime = 1;
        while ((lookOffsetTimer <= recoilTime && lookOffsetInitial.x > lookOffsetTarget.x) || (lookOffsetTimer <= returnTime && lookOffsetInitial.x < lookOffsetTarget.x)) 
        {
            lookOffsetTimer += Time.deltaTime;
            lookOffsetCurrent = Vector3.Lerp(lookOffsetInitial, lookOffsetTarget, lookOffsetTimer/recoilTime);
            if (lookOffsetInitial.x > lookOffsetTarget.x)
            {
                lookOffsetCurrent = Vector3.Lerp(lookOffsetInitial, lookOffsetTarget, lookOffsetTimer / recoilTime);
                if (lookOffsetTimer >= recoilTime)
                {
                    lookOffsetTarget = Vector3.zero;
                    lookOffsetInitial = lookOffsetCurrent;
                    returnTime = Mathf.Log((lookOffsetInitial - lookOffsetTarget).magnitude * recoilReturnTimeMultiplier + 1);
                    Debug.Log(returnTime);
                    lookOffsetTimer = 0;
                }
            }
            else 
            {
                lookOffsetCurrent = Vector3.Lerp(lookOffsetInitial, lookOffsetTarget, lookOffsetTimer / returnTime);
            }
            yield return null;
        }
        lookOffsetCurrent = Vector3.zero;
        recoilCoroutine = null;
    }
}
