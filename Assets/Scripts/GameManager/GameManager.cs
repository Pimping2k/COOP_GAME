using System;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

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
            NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
        }
    }
}