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

        [SerializeField] Image healthFill;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] float fillSpeed = 0.01f;

        public void SetHealth(Health health)
        {
            this.health = health;
            healthFill.fillAmount = health.GetHealthPercent();
        }

        private void UpdateHealth()
        {
            // gameObject is disabled before this function is called
            if (healthFill == null)
            {
                Debug.Log("Health Fill is Null");
                return;
            }
            healthFill.fillAmount = health.GetHealthPercent();
            healthText.text = $"{(int)health.GetHealthPoint()} / {(int)health.GetMaxHealth()}";
            //healthFill.fillAmount = Mathf.MoveTowards(currentHealthFill, targetHealthFill, fillSpeed * Time.deltaTime);
        }
    }

}