using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TMPNetworkButtons : MonoBehaviour
{
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button ClientButton;

    public void Awake()
    {
        HostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        ServerButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        ClientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }
}
