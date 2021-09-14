using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float radius = 0.5f;

    public float GetRadius()
    {
        return radius;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
}
