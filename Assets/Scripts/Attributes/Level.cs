using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;



public class Level : NetworkBehaviour
{
    public const float LEVEL_UP_THRESHOLD = 100f;

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
        statStore.SetStats(progression.GetStats(currentLevel));
    }



}
