using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAreaIndicator : NetworkBehaviour
{
    [SerializeField] Canvas areaCanvas = null;

    [SyncVar(hook = nameof(OnRadiusUpdated))]
    float radius;

    #region Server
    public void ServerSetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void ServerSetRadius(float radius)
    {
        this.radius = radius;
        RectTransform rt = areaCanvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(this.radius * 2, this.radius * 2);
    }
    #endregion

    #region Client
    private void OnRadiusUpdated(float oldRadius, float newRadius)
    {
        RectTransform rt = areaCanvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(newRadius * 2, newRadius * 2);
    }
    #endregion
}
