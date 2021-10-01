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
        left.attackSpeed += right.attackSpeed;
        left.magicDamage += right.magicDamage;
        left.armor += right.armor;
        left.magicResist += right.magicResist;
        left.attackRange += right.attackRange;
        left.cooldownReduction += right.cooldownReduction;
        return left;
    }

    public static Stats operator -(Stats left, Stats right)
    {
        left.maxHealth -= right.maxHealth;
        left.maxMana -= right.maxMana;
        left.manaRegen -= right.manaRegen;
        left.healthRegen -= right.healthRegen;
        left.attackSpeed -= right.attackSpeed;
        left.magicDamage -= right.magicDamage;
        left.armor -= right.armor;
        left.magicResist -= right.magicResist;
        left.attackRange -= right.attackRange;
        left.cooldownReduction -= right.cooldownReduction;
        return left;
    }

    public float maxHealth;
    public float maxMana;
    public float manaRegen;
    public float healthRegen;
    public float attackSpeed;
    public float moveSpeed;
    public float attackDamage;
    public float magicDamage;
    public float armor;
    public float magicResist;
    public float attackRange;
    public float cooldownReduction;
}
