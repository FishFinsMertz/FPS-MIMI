using UnityEngine;

public struct JetpackStart : IEvent 
{
    public GameObject jetpackOwner;
}
public struct JetpackEnd : IEvent
{
    public GameObject jetpackOwner;
}

public class JetpackComponent : MonoBehaviour, IComponent
{
    EventBinding<JetpackStart> jetPackStartEventBinding;
    EventBinding<JetpackEnd> jetPackEndEventBinding;

    [Header("Jetpack Specs")]
    [SerializeField] float fuelSize;
    [SerializeField] float jetpackStrength = 10;
    [SerializeField] float jetpackMaxSpeed = 10;

    [Header("Jetpack Debug")]
    [SerializeField] bool isJetpacking;
    [SerializeField] float curFuel;

    private LocalEventBusManager localEventBusManager;

    private void OnEnable()
    {
        jetPackStartEventBinding = new EventBinding<JetpackStart>(handleJetPackStartEvent);
        jetPackEndEventBinding = new EventBinding<JetpackEnd>(handleJetPackEndEvent);
        if (localEventBusManager != null)
        {
            Initialize(localEventBusManager);
        }
    }

    private void OnDisable()
    {
        if (localEventBusManager != null)
        {
            localEventBusManager.GetLocalEventBus<JetpackStart>().Deregister(jetPackStartEventBinding);
            localEventBusManager.GetLocalEventBus<JetpackEnd>().Deregister(jetPackEndEventBinding);
        }
    }

    void FixedUpdate()
    {
        if (isJetpacking && curFuel > 0) 
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            Vector3 velocity = rb.linearVelocity;
            if (rb.linearVelocity.y < jetpackMaxSpeed)
            {
                velocity.y = Mathf.Clamp(velocity.y + jetpackStrength * Time.deltaTime, velocity.y, jetpackMaxSpeed);
                rb.linearVelocity = velocity;
            }
            curFuel -= Time.deltaTime;
        }
        else if (!isJetpacking)
        {
            curFuel += Time.deltaTime;
            Mathf.Clamp(curFuel, 0, fuelSize);
        }
    }

    void handleJetPackStartEvent(JetpackStart e) 
    {
        if (e.jetpackOwner != gameObject) return;
        isJetpacking = true;
    }
    void handleJetPackEndEvent(JetpackEnd e) 
    {
        if (e.jetpackOwner != gameObject) return;
        isJetpacking = false;
    }

    public void Initialize(LocalEventBusManager LEBM) 
    {
        localEventBusManager = LEBM;
        Debug.Log("JetPack initialized");
        localEventBusManager.GetLocalEventBus<JetpackStart>().Register(jetPackStartEventBinding, true);
        localEventBusManager.GetLocalEventBus<JetpackEnd>().Register(jetPackEndEventBinding, true);
    }
}
