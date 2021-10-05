using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stats
{
    public static Stats operator +(Stats left, Stats right)
    {
        left.maxHealth += right.maxHealth;
        left.maxMana += right.maxMana;
        left.manaRegen += right.manaRegen;
        left.healthRegen += right.healthRegen;
        left.armor += right.armor;
        return left;
    }

    public static Stats operator -(Stats left, Stats right)
    {
        left.maxHealth -= right.maxHealth;
        left.maxMana -= right.maxMana;
        left.manaRegen -= right.manaRegen;
        left.healthRegen -= right.healthRegen;
        left.armor -= right.armor;
        return left;
    }

    public float maxHealth;
    public float maxMana;
    public float manaRegen;
    public float healthRegen;
    public float moveSpeed;
    public float attackDamage;
    public float armor;
}
