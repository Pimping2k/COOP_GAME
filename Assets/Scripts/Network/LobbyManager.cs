using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private List<Transform> playerSpawnPointParent;
    [SerializeField] private GameObject playerModelPrefab;

    private List<GameObject> spawnedModels = new List<GameObject>();

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

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
    }

    public void OnHostCreated()
    {
        SpawnPlayerModelOnPedestal(NetworkManager.Singleton.LocalClientId, 0);
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
    }

    public void OnClientConnected()
    {
        OnPlayerConnected(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPlayerModelOnPedestal(NetworkManager.Singleton.LocalClientId, 0);
        }
    }

    private void OnPlayerConnected(ulong clientId)
    {
        if (!IsServer)
            return;

        int pedestalIndex = spawnedModels.Count;

        if (pedestalIndex < playerSpawnPointParent.Count)
        {
            SpawnPlayerModelOnPedestal(clientId, pedestalIndex);
        }
    }

    private void SpawnPlayerModelOnPedestal(ulong clientId, int pedestalIndex)
    {
        Transform spawnPoint = playerSpawnPointParent[pedestalIndex];

        if (spawnedModels.Exists(m => m.GetComponent<NetworkObject>().OwnerClientId == clientId))
            return;

        GameObject modelInstance = Instantiate(playerModelPrefab, spawnPoint.position, spawnPoint.rotation);
        modelInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

        spawnedModels.Add(modelInstance);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
    }
}