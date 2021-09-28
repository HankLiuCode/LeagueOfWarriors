using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{

    CanvasGroup cg;
    
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void ChangeAlpha(Slider s)
    {
        cg.alpha = s.value;
    }

}