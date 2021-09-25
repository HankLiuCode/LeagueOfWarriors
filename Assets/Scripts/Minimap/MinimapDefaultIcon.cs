using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapDefaultIcon : MinimapIcon
{
    [SerializeField] GameObject icon = null;
    [SerializeField] Image teamRepresentImage = null;
    [SerializeField] Sprite redTeamSprite = null;
    [SerializeField] Sprite blueTeamSprite = null;
    [SerializeField] Sprite noneTeamSprite = null;
    [SerializeField] bool isVisible;

    public override void SetTeam(Team team)
    {
        switch (team)
        {
            case Team.Red:
                teamRepresentImage.sprite = redTeamSprite;
                break;

            case Team.Blue:
                teamRepresentImage.sprite = blueTeamSprite;
                break;

            case Team.None:
                teamRepresentImage.sprite = noneTeamSprite;
                break;
        }
    }

    public override void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        icon.SetActive(isVisible);
    }
}
