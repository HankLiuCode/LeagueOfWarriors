using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Player State/Data")]
public class PlayerStateData : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    
    public int maxMana;
    public int currentMana;

    public int basePhysicalDefence;
    public int currentPhysicalDefence;
    public int baseManaDefence;
    public int currentManaDefence;

    public int basePhysicalAttack;
    public int currentPhysicalAttack;
    public int baseManaAttack;
    public int currentManaAttack;

    public int speed;


}
