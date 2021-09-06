using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField] GameObject playerIcon = null;
    [SerializeField] Image teamBorder = null;
    [SerializeField] Image playerFace = null;

    [SerializeField] bool isVisible;
    
    public void SetVisible(bool isVisible)
    {
        this.isVisible = isVisible;
        playerIcon.SetActive(isVisible);
    }

    public void SetTeam(Team team)
    {
        switch (team)
        {
            case Team.Red:
                teamBorder.color = Color.red;
                break;

            case Team.Blue:
                teamBorder.color = Color.blue;
                break;
        }
    }

    public void SetPlayerIcon(Sprite sprite)
    {
        playerFace.sprite = sprite;
    }
}
