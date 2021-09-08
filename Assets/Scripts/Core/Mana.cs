using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;

public class Mana : NetworkBehaviour
{
    [SerializeField]
    [SyncVar(hook = nameof(OnManaChanged))]
    float manaPoint;

    [SerializeField] Stats stats = null;

    public event System.Action OnManaModified;


    #region Both
    public float GetMana()
    {
        return manaPoint;
    }

    public float GetManaPercent()
    {
        return manaPoint / stats.GetMaxMana();
    }

    public float GetManaPoint()
    {
        return manaPoint;
    }

    public float GetMaxMana()
    {
        return stats.GetMaxMana();
    }

    #endregion
    
    #region Server
    [ServerCallback]
    private void Update()
    {
        if (manaPoint < stats.GetMaxMana())
        {
            manaPoint += stats.GetManaRegenRate() * Time.deltaTime;
            if (manaPoint > stats.GetMaxMana())
            {
                manaPoint = stats.GetMaxMana();
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

    public override void OnStartClient()
    {
        manaPoint = stats.GetMaxMana();
    } 

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

    private void OnManaChanged(float oldValue, float newValue)
    {
        OnManaModified?.Invoke();
    }
    #endregion
}
