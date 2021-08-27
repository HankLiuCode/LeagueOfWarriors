using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mana : NetworkBehaviour
{
    [SyncVar]
    float manaPoint = 100f;

    [SyncVar]
    float maxManaPoint = 100f;

    [SyncVar]
    float regenRate = 1f;


    #region Both
    public float GetMaxMana()
    {
        return maxManaPoint;
    }

    public float GetMana()
    {
        return manaPoint;
    }

    public float GetManaPercent()
    {
        return manaPoint / GetMaxMana();
    }

    public float GetRegenRate()
    {
        return regenRate;
    }
    #endregion


    #region Server
    [ServerCallback]
    private void Update()
    {
        if (manaPoint < GetMaxMana())
        {
            manaPoint += GetRegenRate() * Time.deltaTime;
            if (manaPoint > GetMaxMana())
            {
                manaPoint = GetMaxMana();
            }
        }
    }

    [Server]
    public bool ServerUseMana(float manaToUse)
    {
        if (manaToUse > manaPoint)
        {
            return false;
        }
        manaPoint -= manaToUse;
        return true;
    }

    [Command]
    public void CmdUseMana(float manaToUse)
    {
        ServerUseMana(manaToUse);
    }

    #endregion

    #region Client

    [Client]
    public bool IsManaEnough(float manaToUse)
    {
        if (manaToUse > manaPoint)
        {
            return false;
        }
        return true;
    }

    [Client]
    public void ClientUseMana(float manaToUse)
    {
        CmdUseMana(manaToUse);
    }
    #endregion
}
