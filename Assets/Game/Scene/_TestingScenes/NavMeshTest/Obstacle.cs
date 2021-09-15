using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float radius = 0.5f;

    [SerializeField] private bool isEnabled = true;

    public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; } }

    public float GetRadius()
    {
        return radius;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
}
