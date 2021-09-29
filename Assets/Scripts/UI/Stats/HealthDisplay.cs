using Dota.Attributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Dota.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        // for debugging
        [SerializeField] Health health;

        [SerializeField] Image healthFill = null;
        [SerializeField] TextMeshProUGUI healthText = null;
        [SerializeField] float fillSpeed = 0.005f;

        float targetHealthFill = 1f;

        public void SetHealth(Health health)
        {
            if(this.health != null)
            {
                this.health.ClientOnHealthModified -= Health_OnClientHealthModified;
            }

            this.health = health;
            this.health.ClientOnHealthModified += Health_OnClientHealthModified;
            targetHealthFill = health.GetHealthPercent();
        }

        private void Update()
        {
            float currentHealthFill = healthFill.fillAmount;
            healthFill.fillAmount = Mathf.MoveTowards(currentHealthFill, targetHealthFill, fillSpeed * Time.deltaTime);
        }

        private void Health_OnClientHealthModified(float oldVal, float newVal)
        {
            UpdateHealth(oldVal, newVal);
        }

        private void UpdateHealth(float oldVal, float newVal)
        {
            // gameObject is disabled before this function is called
            if(healthFill == null) 
            {
                Debug.Log("Health Fill is Null");
                return; 
            }

            targetHealthFill = newVal / health.GetMaxHealth();
            healthText.text = $"{(int) health.GetHealthPoint()} / {(int) health.GetMaxHealth()}";
        }
    }

}