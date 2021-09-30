using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject toolTip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("·Æ¹«¶i¤J");
        toolTip.SetActive(true);
        toolTip.transform.SetParent(this.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("·Æ¹«Â÷¶}");
        toolTip.SetActive(false);
        toolTip.transform.SetParent(this.gameObject.transform);
    }
}
