using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapEntity : MonoBehaviour
{
    [SerializeField]
    GameObject miniMapPrefab = null;

    
    public GameObject GetMinimapRepresentation(Transform parent)
    {
        MinimapIcon minimapIconInstance = Instantiate(miniMapPrefab, parent).GetComponent<MinimapIcon>();
        //minimapIconInstance.SetTeam(minion.GetTeam());

        return miniMapPrefab;
    }
}
