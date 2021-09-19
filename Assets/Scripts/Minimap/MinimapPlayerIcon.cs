using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayerIcon : MinimapIcon
{
    [SerializeField] Image playerFace = null;
    [SerializeField] GameObject icon = null;
    [SerializeField] Image teamRepresentImage = null;
    [SerializeField] bool isVisible;

    public void SetPlayerIcon(Sprite sprite)
    {
        playerFace.sprite = sprite;
    }

    public override void SetTeam(Team team)
    {
        switch (team)
        {
            case Team.Red:
                teamRepresentImage.color = Color.red;
                break;

            case Team.Blue:
                teamRepresentImage.color = Color.blue;
                break;
        }
    }

    public override void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        icon.SetActive(isVisible);
    }
}
