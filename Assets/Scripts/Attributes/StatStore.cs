using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Attributes
{
    public class StatStore : NetworkBehaviour
    {

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField] 
        float maxHealth;

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField]
        float maxMana;

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField] 
        float movementSpeed;

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField] 
        float attackSpeed;

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField] 
        float damage;

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField] 
        float healthRegenRate;

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField] 
        float manaRegenRate;

        [SyncVar(hook = nameof(OnStatsChanged))]
        [SerializeField] 
        float armor;

        public event System.Action OnStatsModified;


        private void OnStatsChanged(float oldValue, float newValue)
        {
            OnStatsModified?.Invoke();
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public float GetMaxMana()
        {
            return maxMana;
        }

        public float GetManaRegenRate()
        {
            return manaRegenRate;
        }

        public float GetMovementSpeed()
        {
            return movementSpeed;
        }

        public float GetDamage()
        {
            return damage;
        }
    }

}