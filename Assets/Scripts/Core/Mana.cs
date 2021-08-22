using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mana : NetworkBehaviour
{
    [SyncVar]
    float manaPoint = 100f;


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

    #endregion

    public float GetMana()
    {
        return manaPoint;
    }

    public bool UseMana(float manaToUse)
    {
        if (manaToUse > manaPoint)
        {
            return false;
        }

        manaPoint -= manaToUse;
        return true;
    }

    public float GetMaxMana()
    {
        // return from stats
        return 100;
    }

    public float GetManaPercent()
    {
        return manaPoint / GetMaxMana();
    }

    public float GetRegenRate()
    {
        // return from stats
        return 1;
    }
}
