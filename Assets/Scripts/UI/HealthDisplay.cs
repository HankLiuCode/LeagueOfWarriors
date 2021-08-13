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
        [SerializeField] Health health;
        [SerializeField] Image healthFill;

        private void Update()
        {
            if(health == null)
            {
                Health health = NetworkClient.localPlayer.gameObject.GetComponent<Health>();
                this.health = health;
            }

            if (health != null)
            {
                healthFill.fillAmount = health.GetHealthPercent();
            }
        }
    }

}