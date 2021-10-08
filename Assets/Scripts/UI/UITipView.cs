using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipView : MonoBehaviour
{
    [SerializeField] private GameObject TipCanvas;

    public string title;
    public Object tipWord;
    public Image tipWordImage;



    public void SetTipPanel(bool isActive)
    {
        TipCanvas.SetActive(isActive);
    }
}
