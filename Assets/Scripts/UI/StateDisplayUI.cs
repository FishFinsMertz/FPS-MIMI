using UnityEngine;
using TMPro;
using Unity.Netcode;

public class StateDisplayUI : NetworkBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI stateText;
    
    [Header("Display Settings")]
    [SerializeField] private string statePrefix = "State: ";
    
    private PlayerController localPlayerController;
    private EventBinding<LocalPlayerSpawned> playerSpawnEventBinding;

    private void OnEnable()
    {
        playerSpawnEventBinding = new EventBinding<LocalPlayerSpawned>(OnLocalPlayerSpawned);
        EventBus<LocalPlayerSpawned>.Register(playerSpawnEventBinding);
    }

    private void OnDisable()
    {
        EventBus<LocalPlayerSpawned>.Deregister(playerSpawnEventBinding);
        
        if (localPlayerController != null)
        {
            localPlayerController.OnStateChanged -= UpdateStateText;
        }
    }

    private void OnLocalPlayerSpawned(LocalPlayerSpawned playerSpawnedEvent)
    {
        localPlayerController = playerSpawnedEvent.playerController;
        localPlayerController.OnStateChanged += UpdateStateText;
        
        // Set initial state text
        if (stateText != null)
        {
            stateText.text = statePrefix + "Connected";
        }
    }

    private void UpdateStateText(string newState)
    {
        if (stateText != null)
        {
            stateText.text = statePrefix + newState;
        }
    }
} 