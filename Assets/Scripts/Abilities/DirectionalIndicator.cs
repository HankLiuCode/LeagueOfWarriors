using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalIndicator : MonoBehaviour
{
    [SerializeField] Canvas areaCanvas = null;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos = Vector3.one;


    public void SetStartPosition(Vector3 position)
    {
        
    }

    public void SetEndPosition(Vector3 position)
    {

    }

    public void SetDirection(Vector3 direction)
    {

    }

    public void SetLength()
    {

    }

    public void SetWidth(float width)
    {
        RectTransform rt = areaCanvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }
}
