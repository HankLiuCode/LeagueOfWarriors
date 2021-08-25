using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRectIndicator : NetworkBehaviour
{
    [SerializeField] GameObject wrapper = null;

    [SyncVar(hook = nameof(OnDirectionUpdated))]
    [SerializeField] Vector2 direction = Vector2.zero;

    [SyncVar(hook = nameof(OnLengthUpdated))]
    [SerializeField] float length = 1f;

    [SyncVar(hook = nameof(OnWidthUpdated))]
    [SerializeField] float width = 1f;


    #region Server

    public void ServerSetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void ServerSetDirection(Vector3 direction)
    {
        ServerSetDirection(new Vector2(direction.x, direction.z));
    }

    public Vector3 ServerGetDirection()
    {
        return new Vector3(direction.x, 0, direction.y);
    }

    public void ServerSetDirection(Vector2 direction)
    {
        this.direction = direction;
        Quaternion transRot = Quaternion.LookRotation(new Vector3(this.direction.x, 0, this.direction.y));
        transform.rotation = transRot;
    }

    public void ServerSetLength(float length)
    {
        this.length = length;
        Vector3 scale = wrapper.transform.localScale;
        wrapper.transform.localScale = new Vector3(scale.x, scale.y, length);
    }

    public void ServerSetWidth(float width)
    {
        this.width = width;
        Vector3 scale = wrapper.transform.localScale;
        wrapper.transform.localScale = new Vector3(width, scale.y, scale.z);
    }
    #endregion

    #region Client
    private void OnDirectionUpdated(Vector2 oldDirection, Vector2 newDirection)
    {
        Quaternion transRot = Quaternion.LookRotation(new Vector3(newDirection.x, 0, newDirection.y));
        transform.rotation = transRot;
    }

    private void OnLengthUpdated(float oldLength, float newLength)
    {
        Vector3 scale = wrapper.transform.localScale;
        wrapper.transform.localScale = new Vector3(scale.x, scale.y, newLength);
    }

    private void OnWidthUpdated(float oldWidth, float newWidth)
    {
        Vector3 scale = wrapper.transform.localScale;
        wrapper.transform.localScale = new Vector3(newWidth, scale.y, scale.z);
    }
    #endregion
}
