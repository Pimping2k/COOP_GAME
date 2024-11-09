using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private GameObject _camera;
    void Start()
    {
        if (IsLocalPlayer)
        {
            _camera.gameObject.SetActive(true);
        }
        else
        {
            _camera.gameObject.SetActive(false);
        }
    }
}