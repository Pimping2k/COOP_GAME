using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityComponent : MonoBehaviour
{
    private Renderer objectRenderer;

    private void Awake()
    {
        objectRenderer = this.GetComponent<Renderer>();
    }

    private void Start()
    {
        objectRenderer.enabled = false;
    }

    public void SetVisible(bool isVisible)
    {
        if (objectRenderer != null)
            objectRenderer.enabled = isVisible;
    }
}