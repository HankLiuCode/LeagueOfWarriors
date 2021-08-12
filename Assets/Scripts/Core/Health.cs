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

        public void TakeDamage(float damage)
        {
            healthPoint = Mathf.Max(healthPoint - damage, 0);
            if (healthPoint == 0 && !isDead)
            {
                RpcTriggerDeathAnimation();
                isDead = true;
            }
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
        }

        [Server]
        public void ServerHeal(float amount)
        {
            healthPoint = Mathf.Min(healthPoint + amount);
        }
        #endregion


        #region Client
        [Command]
        public void CmdTakeDamage(float damage)
        {
            ServerTakeDamage(damage);
        }

        public void ClientTakeDamage(float damage)
        {
            CmdTakeDamage(damage);
        }
        #endregion
    }

}