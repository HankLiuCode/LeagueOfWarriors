using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionEntity : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    bool isVisible;

    public void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        meshRenderer.enabled = isVisible;
    }

    public bool GetVisible()
    {
        return this.isVisible;
    }
}
