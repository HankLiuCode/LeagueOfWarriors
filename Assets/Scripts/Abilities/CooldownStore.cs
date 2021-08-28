using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CooldownStore : NetworkBehaviour
{
    // this syncList is used as an array, !!! DO NOT ADD new elements into this !!!
    SyncDictionary<int, float> cooldownTimers = new SyncDictionary<int, float>();
    SyncDictionary<int, float> initialCooldownTimes = new SyncDictionary<int, float>();


    #region Server

    [ServerCallback]
    private void Update()
    {
        List<int> keys = new List<int>(cooldownTimers.Keys);
        foreach (int abilityId in keys)
        {
            cooldownTimers[abilityId] -= Time.deltaTime;
            if (cooldownTimers[abilityId] < 0)
            {
                cooldownTimers.Remove(abilityId);
                initialCooldownTimes.Remove(abilityId);
            }
        }
    }

    #endregion

    public void StartCooldown(Ability ability, float cooldownTime)
    {
        if (cooldownTimers.ContainsKey(ability.GetInstanceID())) { return; }

        cooldownTimers.Add(ability.GetInstanceID(), cooldownTime);
        initialCooldownTimes.Add(ability.GetInstanceID(), cooldownTime);
    }

    public float GetTimeRemaining(Ability ability)
    {
        if (!cooldownTimers.ContainsKey(ability.GetInstanceID()))
        {
            return 0;
        }
        return cooldownTimers[ability.GetInstanceID()];
    }

    public float GetFractionRemaining(Ability ability)
    {
        if (!cooldownTimers.ContainsKey(ability.GetInstanceID()))
        {
            return 0;
        }

        return cooldownTimers[ability.GetInstanceID()] / initialCooldownTimes[ability.GetInstanceID()];
    }


}
