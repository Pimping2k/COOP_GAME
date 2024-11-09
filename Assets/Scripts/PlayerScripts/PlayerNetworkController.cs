using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private GameObject HUDPrefab;
    private void Start()
    {
        HudOwnerCheck();
    }

    private void HudOwnerCheck()
    {
        if (IsOwner)
        {
            HUDPrefab.gameObject.SetActive(true);
        }
        else
        {
            HUDPrefab.gameObject.SetActive(false);
        }
    }
}
