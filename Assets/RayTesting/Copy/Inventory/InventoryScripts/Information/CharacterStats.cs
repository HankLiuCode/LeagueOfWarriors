using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;

    public int maxMana;
    public int currentMana;

    public int baseDefense;
    public int currentDefence;

}
