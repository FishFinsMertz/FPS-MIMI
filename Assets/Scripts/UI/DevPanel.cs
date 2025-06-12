using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class DevPanel : MonoBehaviour
{
    [Header("Necessary References")]
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject ConsoleMessagePrefab;
    [SerializeField] private InputActionReference toggleKey;

    [Header("Specs")]
    [SerializeField] private float consolePaddingRate;

    private List<GameObject> messageObjects = new List<GameObject>();
    private List<string> messages = new List<string>();
    private PlayerInput playerInput;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Content = transform.GetChild(0).GetChild(0);
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction(toggleKey.action.id).performed += ctx => toggleDevPanel();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        SendMessage(" (" + type.ToString() + "): " + logString, "[SYSTEM]");
    }

    public void SendMessage(string message, string sender) 
    {
        GameObject msgObj = Instantiate(ConsoleMessagePrefab, Content);
        messageObjects.Add(msgObj);
        TMP_Text msgText = msgObj.GetComponent<TMP_Text>();

        string formattedMessage = sender + " " + message;
        msgText.text = formattedMessage;

        msgText.ForceMeshUpdate();
        float preferredHeight = msgText.textBounds.size.y;
        msgObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight + msgText.textInfo.lineInfo[0].lineHeight * consolePaddingRate);

        messages.Add(formattedMessage);
    }
    
    private void toggleDevPanel()
    {
        canvasGroup.alpha = canvasGroup.alpha == 0 ? 1 : 0;
    }
}
