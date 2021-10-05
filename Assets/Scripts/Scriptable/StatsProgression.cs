using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatsProgression")]
public class StatsProgression : ScriptableObject
{
    [SerializeField] Stats[] stats;

    public Stats GetStats(int level)
    {
        if(level <= 0) 
        { 
            return stats[0]; 
        }
        else if(level > 0 && level <= stats.Length)
        {
            return stats[level - 1];
        }
        else
        {
            return stats[stats.Length - 1];
        }
    }
}
