using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectClientButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI joinCodeText;

    private void Start()
    {
        hostButton.onClick.AddListener(CreateHost);
        connectClientButton.onClick.AddListener(ConnectClient);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private async void CreateHost()
    {
        string joinCode = await RelayManager.Instance.CreateRelay(4);

        if (!string.IsNullOrEmpty(joinCode))
        {
            Debug.Log("Session is created. Join Code: " + joinCode);
            joinCodeText.text = joinCode;
            joinCodeInputField.text = joinCode;

            connectClientButton.interactable = false;

            LobbyManager.Instance.OnHostCreated();
        }
    }

    private async void ConnectClient()
    {
        string joinCode = joinCodeInputField.text;

        if (!string.IsNullOrEmpty(joinCode))
        {
            await RelayManager.Instance.JoinRelay(joinCode);
            Debug.Log("Connection is finished!");
            LobbyManager.Instance.OnClientConnected();
        }
        else
        {
            Debug.LogError("Input join code for connection");
        }
    }
}