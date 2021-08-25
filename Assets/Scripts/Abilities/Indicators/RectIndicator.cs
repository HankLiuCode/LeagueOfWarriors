using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RectIndicator : MonoBehaviour
{
    [SerializeField] GameObject wrapper = null;
    [SerializeField] Vector2 direction = new Vector2(0, 1);
    [SerializeField] float length = 1f;
    [SerializeField] float width = 1f;
    
    public Vector3 GetDirection()
    {
        return new Vector3(direction.x, 0, direction.y);
    }

    public float GetLength()
    {
        return length;
    }

    public float GetWidth()
    {
        return width;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetDirection(Vector3 direction)
    {
        SetDirection(new Vector2(direction.x, direction.z));
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
        Quaternion transRot = Quaternion.LookRotation(new Vector3(this.direction.x, 0, this.direction.y));
        transform.rotation = transRot;
    }

    public void SetLength(float length)
    {
        this.length = length;
        Vector3 scale = wrapper.transform.localScale;
        wrapper.transform.localScale = new Vector3(scale.x, scale.y, length);
    }

    public void SetWidth(float width)
    {
        this.width = width;
        Vector3 scale = wrapper.transform.localScale;
        wrapper.transform.localScale = new Vector3(width, scale.y, scale.z);
    }
}
