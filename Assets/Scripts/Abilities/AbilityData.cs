using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityData
{
    GameObject user;
    Vector3 targetedPoint;
    float radius;
    LayerMask layerMask;
    IEnumerable<GameObject> targets;
    bool success;

    public AbilityData(GameObject user)
    {
        this.user = user;
    }

    public GameObject GetUser()
    {
        return user;
    }

    public void SetLayerMask(LayerMask layerMask)
    {
        this.layerMask = layerMask;
    }

    public LayerMask GetLayerMask()
    {
        return layerMask;
    }

    public void SetSuccess(bool s)
    {
        success = s;
    }

    public bool GetSuccess()
    {
        return success;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    public float GetRadius()
    {
        return radius;
    }

    public void SetTargets(IEnumerable<GameObject> targets)
    {
        this.targets = targets;
    }

    public IEnumerable<GameObject> GetTargets()
    {
        return targets;
    }

    public void SetTargetedPoint(Vector3 targetedPoint)
    {
        this.targetedPoint = targetedPoint;
    }

    public Vector3 GetTargetedPoint()
    {
        return targetedPoint;
    }

    public void StartCoroutine(IEnumerator coroutine)
    {
        user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
    }
}
