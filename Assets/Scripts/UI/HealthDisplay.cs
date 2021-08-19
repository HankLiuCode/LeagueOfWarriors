using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dota.Core;
using Mirror;


namespace Dota.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] Image healthFill;
        
        Health health;
        
        public void SetHealth(Health health)
        {
            this.health = health;
        }

        private void Update()
        {
            if (health != null)
            {
                healthFill.fillAmount = health.GetHealthPercent();
            }
        }
    }

}