using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinimapIcon : MonoBehaviour
{
    public abstract void SetVisible(bool isVisible);
    public abstract void SetTeam(Team team);
}
