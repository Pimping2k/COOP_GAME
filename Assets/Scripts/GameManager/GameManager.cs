using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerHostPrefab;
    [SerializeField] private GameObject playerClientPrefab;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void GameStart()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene(SceneContainer.Lobby, LoadSceneMode.Single);
            NetworkManager.SceneManager.OnLoadComplete += OnSceneLoadComplete;
        }
    }

    private void OnSceneLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode)
    {
        if (IsServer)
        {
            SpawnPlayers();
            NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
        }
    }

    private void SpawnPlayers()
    {
        foreach (var connectedClient in NetworkManager.ConnectedClients)
        {
            GameObject playerInstance;

            if (connectedClient.Key == NetworkManager.LocalClientId)
            {
                playerInstance = Instantiate(playerHostPrefab);
            }
            else
            {
                playerInstance = Instantiate(playerClientPrefab);
            }

            var playerNetworkObject = playerInstance.GetComponent<NetworkObject>();
            playerNetworkObject.SpawnAsPlayerObject(connectedClient.Key);
        }
    }
}