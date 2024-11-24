using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> LobbyPlayers;
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
        }
    }

    public void OnHostCreated()
    {
        if (IsServer)
        {
            ShowPlayersPrefabServerRpc();
        }
    }

    public void OnPlayerConnected(ulong clientId)
    {
        if (IsServer)
        {
            ShowPlayersPrefabServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowPlayersPrefabServerRpc()
    {
        if (!IsServer)
            return;
        
        int connectedClientsCount = NetworkManager.Singleton.ConnectedClients.Count;
        UpdateLobbyPlayersPrefabClientRpc(connectedClientsCount);
    }

    [ClientRpc]
    private void UpdateLobbyPlayersPrefabClientRpc(int connectedPlayersCount)
    {
        if (!IsClient)
            return;
        
        for (int i = 0; i < LobbyPlayers.Count; i++)
        {
            LobbyPlayers[i].SetActive(i < connectedPlayersCount);
        }
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