using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public PlayerStateData playerStateData;

    #region Read from Data

    //血量
    public int MaxHealth
    {
        get { if (playerStateData != null)  return playerStateData.maxHealth;  else return 0; }
        set { playerStateData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get { if (playerStateData != null)  return playerStateData.currentHealth;  else return 0; }
        set { playerStateData.currentHealth = value; }
    }

    //魔力
    public int MaxMana
    {
        get { if (playerStateData != null)  return playerStateData.maxMana;  else return 0; }
        set { playerStateData.maxMana = value; }
    }
    public int CurrentMana
    {
        get { if (playerStateData != null)  return playerStateData.currentMana;  else return 0; }
        set { playerStateData.currentMana = value; }
    }

    //物防.魔防
    public int BasePhysicalDefence
    {
        get { if (playerStateData != null)  return playerStateData.basePhysicalDefence;  else return 0; }
        set { playerStateData.basePhysicalDefence = value; }
    }
    public int CurrentPhysicalDefence
    { 
        get { if (playerStateData != null)  return playerStateData.currentPhysicalDefence;  else return 0; }
        set { playerStateData.currentPhysicalDefence = value; }
    }
    public int BaseManaDefence
    {
        get { if (playerStateData != null)  return playerStateData.baseManaDefence;  else return 0; }
        set { playerStateData.baseManaDefence = value; }
    }
    public int CurrentManaDefence
    {
        get { if (playerStateData != null)  return playerStateData.currentManaDefence;  else return 0; }
        set { playerStateData.currentManaDefence = value; }
    }

    //物攻.魔攻
    public int BasePhysicalAttack
    {
        get { if (playerStateData != null)  return playerStateData.basePhysicalAttack;  else return 0; }
        set { playerStateData.basePhysicalAttack = value; }
    }
    public int CurrentPhysicalAttack
    {
        get { if (playerStateData != null)  return playerStateData.currentPhysicalAttack;  else return 0; }
        set { playerStateData.currentPhysicalAttack = value; }
    }
    public int BaseManaAttack
    {
        get { if (playerStateData != null)  return playerStateData.baseManaAttack;  else return 0; }
        set { playerStateData.baseManaAttack = value; }
    }
    public int CurrentManaAttack
    {
        get { if (playerStateData != null)  return playerStateData.currentManaAttack;  else return 0; }
        set { playerStateData.currentManaAttack = value; }
    }

    //跑速
    public int Speed
    {
        get { if (playerStateData != null)  return playerStateData.speed;  else return 0; }
        set { playerStateData.speed = value; }
    }
    #endregion
}
