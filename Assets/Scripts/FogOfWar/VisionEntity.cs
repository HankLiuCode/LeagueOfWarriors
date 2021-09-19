using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionEntity : MonoBehaviour
{
    [SerializeField] List<GameObject> renderers;
    [SerializeField] float viewRadius = 15f;
    [SerializeField] bool isAlwaysVisible = false;
    public bool IsAlwaysVisible { get { return isAlwaysVisible; } }

    bool isVisible = true;

    public float GetViewRadius()
    {
        return viewRadius;
    }

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
