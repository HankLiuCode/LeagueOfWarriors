using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dota.Attributes;
using TMPro;


namespace Dota.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        // for debugging
        [SerializeField] Health health;

        [SerializeField] Image healthFill = null;
        [SerializeField] TextMeshProUGUI healthText = null;
        
        public void SetHealth(Health health)
        {
            if(this.health != null)
            {
                this.health.OnHealthModified -= Health_OnHealthModified;
            }

            this.health = health;
            this.health.OnHealthModified += Health_OnHealthModified;
            UpdateHealth();
        }

        private void Health_OnHealthModified()
        {
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            healthFill.fillAmount = health.GetHealthPercent();
            healthText.text = $"{(int) health.GetHealthPoint()} / {(int) health.GetMaxHealth()}";
        }
    }

}