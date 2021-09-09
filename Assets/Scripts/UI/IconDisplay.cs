using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconDisplay : MonoBehaviour
{
    [SerializeField] Image iconImage;

    public void SetIcon(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }
}
