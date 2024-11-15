using Unity.Netcode;
using UnityEngine;

public class PlayerMeshChanger : NetworkBehaviour
{
    private MeshFilter playerMeshFilter;

    private void Start()
    {
        playerMeshFilter = GetComponent<MeshFilter>();
        NetworkObject networkObject = GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError("NetworkObject не найден на объекте!");
        }

        if (playerMeshFilter == null)
        {
            Debug.LogError("MeshFilter не найден на объекте!");
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ChangeMeshServerRpc(int meshIndex)
    {
        if (!IsServer)
            return;

        playerMeshFilter.sharedMesh = LobbyManager.Instance.AvailableMeshes[meshIndex].sharedMesh;
        ChangeMeshOnClientRpc(meshIndex);
    }

    [ClientRpc]
    private void ChangeMeshOnClientRpc(int meshIndex)
    {
        if (playerMeshFilter != null)
        {
            playerMeshFilter.sharedMesh = LobbyManager.Instance.AvailableMeshes[meshIndex].sharedMesh;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        LobbyManager.Instance.InstanceList[OwnerClientId] = gameObject;
    }
}