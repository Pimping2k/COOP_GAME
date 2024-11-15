using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private List<Transform> playerSpawnPointParent;
    [SerializeField] private GameObject playerModelPrefab;
    [SerializeField] private List<MeshFilter> availableMeshes;

    public List<MeshFilter> AvailableMeshes
    {
        get => availableMeshes;
        set => availableMeshes = value;
    }

    public GameObject PlayerModelPrefab
    {
        get => playerModelPrefab;
        set => playerModelPrefab = value;
    }

    public Dictionary<ulong, GameObject> InstanceList { get; } = new Dictionary<ulong, GameObject>();

    public static LobbyManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
            SpawnPlayerModelOnPedestal(NetworkManager.Singleton.LocalClientId, 0);
        }
    }

    public void OnHostCreated()
    {
        if (IsServer)
        {
            SpawnPlayerModelOnPedestal(NetworkManager.Singleton.LocalClientId, 0);
        }
    }

    public void OnPlayerConnected(ulong clientId)
    {
        if (!IsServer) return;

        int pedestalIndex = InstanceList.Count;

        if (pedestalIndex < playerSpawnPointParent.Count)
        {
            SpawnPlayerModelOnPedestal(clientId, pedestalIndex);
        }
        else
        {
            Debug.LogError("Нет доступных пьедесталов для спавна нового игрока.");
        }
    }

    private void SpawnPlayerModelOnPedestal(ulong clientId, int pedestalIndex)
    {
        Transform spawnPoint = playerSpawnPointParent[pedestalIndex];

        if (InstanceList.ContainsKey(clientId))
            return;

        GameObject modelInstance = Instantiate(playerModelPrefab, spawnPoint.position, spawnPoint.rotation);

        InstanceList[clientId] = modelInstance;

        NetworkObject networkObject = modelInstance.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(clientId);

        Debug.Log($"Игрок с clientId {clientId} спавнен на пьедестале {pedestalIndex}.");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
        }
    }
}