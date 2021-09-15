using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapTowerIcon : MinimapIcon
{
    [SerializeField] GameObject icon = null;
    [SerializeField] Image teamRepresentImage = null;
    [SerializeField] Sprite redTurretSprite = null;
    [SerializeField] Sprite blueTurretSprite = null;
    [SerializeField] bool isVisible;

    public override void SetTeam(Team team)
    {
        switch (team)
        {
            case Team.Red:
                teamRepresentImage.sprite = redTurretSprite;
                break;

            case Team.Blue:
                teamRepresentImage.sprite = blueTurretSprite;
                break;
        }
    }

    public override void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        icon.SetActive(isVisible);
    }
}
