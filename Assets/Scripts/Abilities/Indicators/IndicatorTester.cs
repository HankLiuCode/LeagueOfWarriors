using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorTester : NetworkBehaviour
{
    [SerializeField] NetworkRectIndicator nri = null;
    [SerializeField] Vector2 direction = Vector2.one;
    [SerializeField] float length = 1f;
    [SerializeField] float width = 1f;

    [ServerCallback]
    void Update()
    {
        nri.ServerSetDirection(direction);
        nri.ServerSetLength(length);
        nri.ServerSetPosition(transform.position);
        nri.ServerSetWidth(width);
    }
}
