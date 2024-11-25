using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleItemManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> hostVisibleItems;
    [SerializeField] private List<GameObject> clientVisibleItems;
    
    public static VisibleItemManager Instance;

    public List<GameObject> HostVisibleItems => hostVisibleItems;

    public List<GameObject> ClientVisibleItems => clientVisibleItems;

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
}
