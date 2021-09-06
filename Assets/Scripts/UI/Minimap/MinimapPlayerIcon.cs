using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayerIcon : MinimapIcon
{
    [SerializeField] Image playerFace = null;

    public void SetPlayerIcon(Sprite sprite)
    {
        playerFace.sprite = sprite;
    }
}
