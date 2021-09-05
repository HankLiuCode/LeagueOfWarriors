using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] bool isVisible;


    public void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        gameObject.SetActive(isVisible);
    }

    public void SetIcon(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
