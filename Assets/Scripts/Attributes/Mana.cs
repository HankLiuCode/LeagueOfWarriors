using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Attributes;

public class Mana : NetworkBehaviour
{
    [SerializeField]
    [SyncVar(hook = nameof(OnManaChanged))]
    float manaPoint;

    [SerializeField] StatStore statStore = null;

    public event System.Action OnManaModified;


    #region Both
    public float GetMana()
    {
        return manaPoint;
    }

    public float GetManaPercent()
    {
        return manaPoint / statStore.GetStats().maxMana;
    }

    public float GetManaPoint()
    {
        return manaPoint;
    }

    public float GetMaxMana()
    {
        return statStore.GetStats().maxMana;
    }

    #endregion
    
    #region Server
    [ServerCallback]
    private void Update()
    {
        if (manaPoint < statStore.GetStats().maxMana)
        {
            manaPoint += statStore.GetStats().manaRegen * Time.deltaTime;
            if (manaPoint > statStore.GetStats().maxMana)
            {
                manaPoint = statStore.GetStats().maxMana;
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

    [Client]
    public void ClientUseMana(float manaToUse)
    {
        CmdUseMana(manaToUse);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        manaPoint = statStore.GetStats().maxMana;
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

    private void OnManaChanged(float oldValue, float newValue)
    {
        OnManaModified?.Invoke();
    }
    #endregion
}
