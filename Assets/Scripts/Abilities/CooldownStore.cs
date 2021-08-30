using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


// using GetInstanceID as Key might cause potential bug, check first if bug occurs

public class CooldownStore : NetworkBehaviour
{
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

    [Server]
    public void ServerStartCooldown(Ability ability, float cooldownTime)
    {
        ServerStartCooldown(ability.GetInstanceID(), cooldownTime);
    }

    [Server]
    public void ServerStartCooldown(int abilityID, float cooldownTime)
    {
        if (cooldownTimers.ContainsKey(abilityID)) { return; }

        cooldownTimers.Add(abilityID, cooldownTime);

        initialCooldownTimes.Add(abilityID, cooldownTime);
    }

    [Command]
    private void CmdStartCooldown(int abilityID, float cooldownTime)
    {
        ServerStartCooldown(abilityID, cooldownTime);
    }
    #endregion

    [Client]
    public void ClientStartCooldown(Ability ability, float cooldownTime)
    {
        CmdStartCooldown(ability.GetInstanceID(), cooldownTime);
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
