using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private MeshFilter playerPrefabMeshFilter;

    private void Start()
    {
        playerPrefabMeshFilter = playerPrefab.GetComponent<MeshFilter>();
    }
    
    
}
