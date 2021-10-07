using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;



public class Level : NetworkBehaviour
{
    public const float LEVEL_UP_THRESHOLD = 100f;
    public const int MAX_LEVEL = 30;
    public const int MIN_LEVEL = 1;

    [SerializeField] StatStore statStore = null;
    [SerializeField] private float exp;

    [SyncVar]
    [SerializeField] 
    private int currentLevel = 1;

    [SerializeField] GameObject levelUpEffect = null;

    [SerializeField] StatsProgression progression;


    public int GetLevel()
    {
        return currentLevel;
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
        currentLevel = Mathf.Clamp(currentLevel, MIN_LEVEL, MAX_LEVEL);
        statStore.SetStats(progression.GetStats(currentLevel));
    }


    [Server]
    public void AddExperience(float addExp)
    {
        this.exp += addExp;
        if(this.exp >= LEVEL_UP_THRESHOLD)
        {
            int leveledUp = (int) (this.exp / LEVEL_UP_THRESHOLD);
            currentLevel += leveledUp;
            this.exp = Mathf.Clamp(this.exp, 0, LEVEL_UP_THRESHOLD);

            GameObject levelUpInstance = Instantiate(levelUpEffect, transform.position, Quaternion.identity);
            NetworkServer.Spawn(levelUpInstance);
        }
        SetLevel(currentLevel);
    }



}
