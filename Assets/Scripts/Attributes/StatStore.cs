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
        Stats baseStats;

        List<Stats> statsModifiers = new List<Stats>();
        public event System.Action<Stats> OnStatsModified;
        
        private void OnStatsChanged(Stats oldValue, Stats newValue)
        {
            OnStatsModified?.Invoke(newValue);
        }

        public Stats GetStats()
        {
            return baseStats;
        }
    }
}