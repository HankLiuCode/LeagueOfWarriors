using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapMinionIcon : MinimapIcon
{
    [SerializeField] GameObject icon = null;

    [SerializeField] Image teamRepresentImage = null;

    [SerializeField] bool isVisible;

    public override void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        icon.SetActive(isVisible);
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
}
