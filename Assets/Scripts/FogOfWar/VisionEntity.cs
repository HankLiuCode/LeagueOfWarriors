using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionEntity : MonoBehaviour
{
    [SerializeField] List<GameObject> renderers;
    [SerializeField] float viewRadius = 15f;
    [SerializeField] bool isAlwaysVisible = false;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] Collider currentBush = null;
    public bool IsAlwaysVisible { get { return isAlwaysVisible; } }

    bool isVisible = true;

    public bool IsInGrass()
    {
        if(currentBush != null)
        {
            return true;
        }
        return false;
    }

    public Collider GetCurrentBush()
    {
        return currentBush;
    }

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

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & grassLayer) > 0)
        {
            currentBush = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == currentBush)
        {
            currentBush = null;
        }
    }
}
