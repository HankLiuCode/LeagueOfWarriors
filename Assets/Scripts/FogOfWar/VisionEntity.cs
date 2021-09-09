using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionEntity : MonoBehaviour
{
    [SerializeField] List<GameObject> renderers;
    bool isVisible = true;

    public void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        foreach(GameObject renderer in renderers)
        {
            renderer.SetActive(isVisible);
        }
    }

    public bool GetVisible()
    {
        return this.isVisible;
    }
}
