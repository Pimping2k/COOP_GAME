using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabComponent : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject grabPoint;
    [SerializeField] private float maxDistance;
    [SerializeField] private GameObject grabbedObject;
    private IA_PlayerInputs _playerInputs;

    private void OnEnable()
    {
        _playerInputs = new IA_PlayerInputs();
        _playerInputs.Input.GrabAbility.performed += GrabAbilityOnperformed;
        _playerInputs.Input.GrabAbility.canceled += ReleaseObject;
        _playerInputs.Enable();
    }

    private void OnDisable()
    {
        _playerInputs.Input.GrabAbility.performed -= GrabAbilityOnperformed;
        _playerInputs.Input.GrabAbility.canceled -= ReleaseObject;
        _playerInputs.Disable();
    }

    private void GrabAbilityOnperformed(InputAction.CallbackContext obj)
    {
        if (grabbedObject == null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hitInfo, maxDistance))
            {
                if (hitInfo.collider.CompareTag(TagsContainer.GRABBABLEITEM))
                {
                    grabbedObject = hitInfo.collider.gameObject;

                    var rb = grabbedObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.useGravity = false;
                        rb.isKinematic = true;
                    }

                    grabbedObject.transform.parent = grabPoint.transform;
                    grabbedObject.transform.position = grabPoint.transform.position;
                }
            }
        }
    }

    private void ReleaseObject(InputAction.CallbackContext obj)
    {
        ReleaseObject();
    }
    
    private void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = null;

            var rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            grabbedObject = null;
        }
    }
}