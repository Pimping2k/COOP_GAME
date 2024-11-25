using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class SixthSenseAbilityComponent : NetworkBehaviour
{
    [SerializeField] private bool isOn = false;

    private Coroutine EnableAbilityCoroutine;
    private List<VisibilityComponent> visibilityComponents = new();

    public bool IsOn
    {
        get => isOn;
        set => isOn = value;
    }

    private void Start()
    {
        if (OwnerClientId == 0)
        {
            foreach (var item in VisibleItemManager.Instance.HostVisibleItems)
            {
                var visibilityComponent = item.GetComponent<VisibilityComponent>();
                if (visibilityComponent != null)
                {
                    visibilityComponents.Add(visibilityComponent);
                }
                else
                {
                    Debug.LogWarning($"Объект {item.name} не имеет компонента VisibilityComponent!");
                }
            }
        }
        else
        {
            foreach (var item in VisibleItemManager.Instance.ClientVisibleItems)
            {
                var visibilityComponent = item.GetComponent<VisibilityComponent>();
                if (visibilityComponent != null)
                {
                    visibilityComponents.Add(visibilityComponent);
                }
                else
                {
                    Debug.LogWarning($"Объект {item.name} не имеет компонента VisibilityComponent!");
                }
            }
        }
    }

    public void TurnOn()
    {
        if (isOn || !IsOwner)
            return;

        isOn = true;
        if (EnableAbilityCoroutine == null)
        {
            EnableAbilityCoroutine = StartCoroutine(EnableAbility());
        }

        SetItemsVisibilityServerRpc(true);
    }

    public void Turnoff()
    {
        if (!isOn)
            return;

        isOn = false;

        if (EnableAbilityCoroutine != null)
        {
            StopCoroutine(EnableAbilityCoroutine);
            EnableAbilityCoroutine = null;
        }

        SetItemsVisibilityServerRpc(false);
    }

    private IEnumerator EnableAbility()
    {
        float elapsedTime = 0f;
        float goalTime = 10f;

        while (elapsedTime < goalTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Turnoff();
    }

    [ServerRpc]
    private void SetItemsVisibilityServerRpc(bool isVisible)
    {
        SetItemsVisibilityClientRpc(isVisible, OwnerClientId);
    }

    [ClientRpc]
    private void SetItemsVisibilityClientRpc(bool isVisible, ulong clientId, ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        foreach (var item in visibilityComponents)
        {
            item.SetVisible(isVisible);
        }
    }
}