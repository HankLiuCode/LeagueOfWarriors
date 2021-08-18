using Mirror;
using UnityEngine;

namespace Dota.Core
{
    public class Health : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField]
        float healthPoint = 100f;
        float maxHealthPoint = 100f;

        [SyncVar]
        bool isDead = false;

        [SerializeField] Animator animator = null;


        public float GetHealthPoint()
        {
            return healthPoint;
        }

        public float GetHealthPercent()
        {
            return healthPoint / maxHealthPoint;
        }
        
        public bool IsDead()
        {
            return isDead;
        }

        #region Server
        [ClientRpc]
        private void RpcTriggerDeathAnimation()
        {
            animator.SetTrigger("die");
        }

        [Server]
        public void ServerTakeDamage(float damage)
        {
            healthPoint = Mathf.Max(healthPoint - damage, 0);
            if (healthPoint == 0 && !isDead)
            {
                RpcTriggerDeathAnimation();
                isDead = true;
            }
            Debug.Log("Take Damage" + damage);
        }

        [Server]
        public void ServerHeal(float amount)
        {
            healthPoint = Mathf.Min(healthPoint + amount);
        }

        [Command]
        public void CmdTakeDamage(float damage)
        {
            ServerTakeDamage(damage);
        }

        #endregion
    }

}