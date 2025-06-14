using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Game Design Specs")]
    [SerializeField] private float cameraBaseMultiplier = 20;
    [SerializeField] private Vector3 cameraPlayerOffset = new Vector3(0,1,0);
    [SerializeField] private float cameraXMinRotation = -90;
    [SerializeField] private float cameraXMaxRotation = 90;

    [Header("Player Specs")]
    [SerializeField] private float cameraSensitivity = 1;

    private Camera camera;
    private PlayerController playerController;
    private EventBinding<LocalPlayerSpawned> playerSpawnEventBinding;
    private float cameraXRotation = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main;
    }

    private void OnEnable()
    {
        playerSpawnEventBinding = new EventBinding<LocalPlayerSpawned>(Initialize);
        EventBus<LocalPlayerSpawned>.Register(playerSpawnEventBinding);
    }

    private void OnDisable()
    {
        EventBus<LocalPlayerSpawned>.Deregister(playerSpawnEventBinding);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController) { return; }
        Vector2 lookDir = playerController.GetLookInput();
        camera.transform.Rotate(Vector3.up, lookDir.x * Time.deltaTime * cameraBaseMultiplier * cameraSensitivity, Space.World);
        
        cameraXRotation -= lookDir.y * Time.deltaTime * cameraBaseMultiplier * cameraSensitivity;
        cameraXRotation = Mathf.Clamp(cameraXRotation, cameraXMinRotation, cameraXMaxRotation);
        Vector3 targetRotation = camera.transform.eulerAngles;
        targetRotation.x = cameraXRotation;
        camera.transform.eulerAngles = targetRotation;
        
        camera.transform.position = playerController.transform.position + cameraPlayerOffset;
    }

    void Initialize(LocalPlayerSpawned playerSpawnedEvent) 
    {
        playerController = playerSpawnedEvent.playerController;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
