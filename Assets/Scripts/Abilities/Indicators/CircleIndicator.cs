using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleIndicator : MonoBehaviour
{
    [SerializeField] Canvas areaCanvas = null;

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetRadius(float radius)
    {
        RectTransform rt = areaCanvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(radius * 2, radius * 2);
    }
}
