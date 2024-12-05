using System;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabComponent : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject grabPoint;
    [SerializeField] private float maxDistance;
    private GameObject grabbedObject;
    private IA_PlayerInputs _playerInputs;

    private void OnEnable()
    {
        _playerInputs = new IA_PlayerInputs();
        _playerInputs.Input.GrabAbility.performed += GrabAbilityOnPerformed;
        _playerInputs.Input.GrabAbility.canceled += ReleaseObject;
        _playerInputs.Enable();
    }

    private void OnDisable()
    {
        _playerInputs.Input.GrabAbility.performed -= GrabAbilityOnPerformed;
        _playerInputs.Input.GrabAbility.canceled -= ReleaseObject;
        _playerInputs.Disable();
    }

    private bool isGrabbing;

    private void GrabAbilityOnPerformed(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;

        if (grabbedObject == null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hitInfo, maxDistance))
            {
                if (hitInfo.collider.CompareTag(TagsContainer.GRABBABLEITEM))
                {
                    var networkObject = hitInfo.collider.GetComponent<NetworkObject>();
                    if (networkObject != null)
                    {
                        grabbedObject = hitInfo.collider.gameObject;
                        var rb = grabbedObject.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.useGravity = false;
                            rb.isKinematic = true;
                        }

                        RequestOwnershipServerRpc(networkObject.NetworkObjectId);

                        isGrabbing = true;
                    }
                }
            }
        }
    }
    
    [ServerRpc]
    private void RequestOwnershipServerRpc(ulong objectId, ServerRpcParams rpcParams = default)
    {
        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        if (networkObject != null)
        {
            networkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
        }
    }


    private void ReleaseObject(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;

        if (grabbedObject != null)
        {
            var rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            ReleaseObjectServerRpc(grabbedObject.GetComponent<NetworkObject>().NetworkObjectId);

            grabbedObject = null;
        }

        isGrabbing = false;
    }

    private void Update()
    {
        if (isGrabbing && grabbedObject != null)
        {
            grabbedObject.transform.position = grabPoint.transform.position;

            UpdateObjectPositionServerRpc(
                grabbedObject.GetComponent<NetworkObject>().NetworkObjectId,
                grabPoint.transform.position
            );
        }
    }

    [ServerRpc]
    private void GrabObjectServerRpc(ulong objectId)
    {
        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        if (networkObject != null)
        {
            var rb = networkObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }
    }

    [ServerRpc]
    private void ReleaseObjectServerRpc(ulong objectId)
    {
        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        if (networkObject != null)
        {
            var rb = networkObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
    }

    [ServerRpc]
    private void UpdateObjectPositionServerRpc(ulong objectId, Vector3 position)
    {
        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        if (networkObject != null)
        {
            networkObject.transform.position = position;

            UpdateObjectPositionClientRpc(objectId, position);
        }
    }

    [ClientRpc]
    private void UpdateObjectPositionClientRpc(ulong objectId, Vector3 position)
    {
        if (IsOwner) return;

        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId];
        if (networkObject != null)
        {
            networkObject.transform.position = position;
        }
    }
}
