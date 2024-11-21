using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectClientButton;
    [SerializeField] private Button spawnPlayerButton;
    [SerializeField] private Button nextMeshButton;
    [SerializeField] private Button previousMeshButton;
    [SerializeField] private Button startGameButton;
    [Header("Text")]
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [Header("Image")] 
    [SerializeField] private Image hostImage;
    [SerializeField] private Image clientImage;
    private int index = 0;
    private GameObject playerInstance;

    private void Start()
    {
        hostButton.onClick.AddListener(CreateHost);
        connectClientButton.onClick.AddListener(ConnectClient);
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
}
