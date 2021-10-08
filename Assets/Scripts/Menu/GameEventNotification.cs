using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEventNotification : MonoBehaviour
{
    [SerializeField] IconDisplay slayerIcon;
    [SerializeField] IconDisplay victimIcon;

    public void SetNotification(Sprite slayerSprite, Sprite victimSprite)
    {
        slayerIcon.SetIcon(slayerSprite);
        victimIcon.SetIcon(victimSprite);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
