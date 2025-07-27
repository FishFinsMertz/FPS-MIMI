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
    [SerializeField] private float recoilShakeTime;
    [SerializeField] private int recoilShakePeakNumber;
    [SerializeField] private AnimationCurve recoilShakeCurve;

    [Header("Player Specs")]
    [SerializeField] private float cameraSensitivity = 1;

    private Camera myCamera;
    private PlayerController playerController;
    private Vector3 lookDirection;

    private Coroutine recoilCoroutine = null;
    private Coroutine shakeCoroutine = null;

    private Vector3 lookOffsetInitial = Vector3.zero;
    private Vector3 lookOffsetCurrent = Vector3.zero;
    private Vector3 lookOffsetTarget = Vector3.zero;
    private Vector3 cameraShakeOffset = Vector3.zero;
    private float lookOffsetTimer = 0;

    private EventBinding<LocalPlayerSpawned> playerSpawnEventBinding;
    private EventBinding<ShootAfterFXEvent> shootAfterFXEventBinding;
    private LocalEventBusManager localEventBusManager;

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
        if (localEventBusManager != null) localEventBusManager.GetLocalEventBus<ShootAfterFXEvent>().Register(shootAfterFXEventBinding, true);
    }

    private void OnDisable()
    {
        EventBus<LocalPlayerSpawned>.Deregister(playerSpawnEventBinding);
        if (localEventBusManager != null) localEventBusManager.GetLocalEventBus<ShootAfterFXEvent>().Deregister(shootAfterFXEventBinding);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController) { return; }
        Vector2 lookDir = playerController.GetLookInput();
        lookDirection.x -= lookDir.y * Time.deltaTime * cameraBaseMultiplier * cameraSensitivity;
        lookDirection.x = Mathf.Clamp(lookDirection.x, cameraXMinRotation, cameraXMaxRotation);
        lookDirection.y += lookDir.x * Time.deltaTime * cameraBaseMultiplier * cameraSensitivity;

        Vector3 camDir = lookDirection + lookOffsetCurrent + cameraShakeOffset;
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

        localEventBusManager = playerController.localEventBusManager;
        localEventBusManager.GetLocalEventBus<ShootAfterFXEvent>().Register(shootAfterFXEventBinding, true);
    }

    void Recoil(ShootAfterFXEvent shootAfterFXEvent) 
    {
        Debug.Log("HI");
        lookOffsetInitial = lookOffsetCurrent;
        lookOffsetTarget = lookOffsetInitial + Vector3.left * recoilStrength;
        lookOffsetTimer = 0;
        if (recoilCoroutine == null) recoilCoroutine = StartCoroutine(RecoilCoroutine());
        if (shakeCoroutine == null) shakeCoroutine = StartCoroutine(ScreenShake());
        else 
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ScreenShake());
        }
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
    IEnumerator ScreenShake() 
    {
        float[] peaks = new float[recoilShakePeakNumber * 2];
        int sign = 1;
        float peakTime = recoilShakeTime / (recoilShakePeakNumber * 2);
        float peakTimer = 0;
        for (int i = 1; i <= peaks.Length; i++) 
        {
            peaks[i - 1] = sign * (i % 2) * recoilShakeCurve.Evaluate(i / (float)peaks.Length);
            Debug.Log("Shake"+sign+", "+i+", "+ recoilShakeCurve.Evaluate(i / (float)peaks.Length));
            Vector3 initialOffset = cameraShakeOffset;
            if (i % 2 == 1) sign = -sign;
            while (peakTimer < peakTime)
            {
                cameraShakeOffset = Vector3.Lerp(initialOffset, peaks[i - 1] * Vector3.up, peakTimer / peakTime);
                peakTimer += Time.deltaTime;
                yield return null;
            }
            peakTimer = 0;
        }
    }
}
