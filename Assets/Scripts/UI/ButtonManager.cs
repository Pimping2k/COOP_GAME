using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectClientButton;
    [SerializeField] private Button spawnPlayerButton;
    [SerializeField] private Button nextMeshButton;
    [SerializeField] private Button previousMeshButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI joinCodeText;

    private int index = 0;
    private GameObject playerInstance;

    private void Start()
    {
        hostButton.onClick.AddListener(CreateHost);
        connectClientButton.onClick.AddListener(ConnectClient);
        nextMeshButton.onClick.AddListener(NextMesh);
        previousMeshButton.onClick.AddListener(PreviousMesh);
        startGameButton.onClick.AddListener(OnStartGame);
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

            LobbyManager.Instance.OnPlayerConnected(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.LogError("Input join code for connection");
        }
    }

    private void OnStartGame()
    {
        GameManager.Instance.GameStart();
    }
    
    private void NextMesh()
    {
        ChangeMesh(1);
    }

    private void PreviousMesh()
    {
        ChangeMesh(-1);
    }

    private void ChangeMesh(int direction)
    {
        if (!LobbyManager.Instance.InstanceList.TryGetValue(NetworkManager.Singleton.LocalClientId, out playerInstance))
        {
            Debug.LogError("Игрок не найден в InstanceList. Ожидайте подключения.");
            return;
        }

        index = (index + direction + LobbyManager.Instance.AvailableMeshes.Count) % LobbyManager.Instance.AvailableMeshes.Count;

        var playerMeshChanger = playerInstance.GetComponent<PlayerMeshChanger>();
        if (playerMeshChanger != null)
        {
            playerMeshChanger.ChangeMeshServerRpc(index);
            Debug.Log($"Меш изменен на индекс {index} для игрока {NetworkManager.Singleton.LocalClientId}");
        }
        else
        {
            Debug.LogError("Компонент PlayerMeshChanger не найден на объекте игрока.");
        }
    }
}