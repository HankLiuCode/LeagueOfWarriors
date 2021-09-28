using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSprite : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.clickCount);
        if (eventData.clickCount == 2)
        {
            Transform p = this.transform.parent; // cell;
            if (p != null && p.parent.GetComponent<ShopScript>() != null)
            {
                UIMain2.Instance().putItemToBagPanel(this);
            }
        }
    }

    
    void Start()
    {

    }

  
    void Update()
    {

    }
}