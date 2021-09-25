using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ChampionIdMapping")]
public class ChampionIdMapping : ScriptableObject
{
    public List<ChampionIdPrefab> championPrefabs;

    public Sprite GetIcon(int championId)
    {
        foreach (ChampionIdPrefab si in championPrefabs)
        {
            if (si.championId == championId)
            {
                return si.championIcon;
            }
        }
        return null;
    }

    public GameObject GetPrefab(int championId)
    {
        foreach (ChampionIdPrefab si in championPrefabs)
        {
            if (si.championId == championId)
            {
                return si.championPrefab;
            }
        }
        return null;
    }

}


[System.Serializable]
public struct ChampionIdPrefab
{
    public int championId;
    public Sprite championIcon;
    public GameObject championPrefab;
}