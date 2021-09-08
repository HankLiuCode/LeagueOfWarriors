using Mirror;
using UnityEngine;

namespace Dota.Core
{
    public class Health : NetworkBehaviour
    {
        [SerializeField]
        [SyncVar(hook = nameof(OnHealthChanged))]
        float healthPoint = 100f;

        [SyncVar]
        bool isDead = false;

        [SerializeField] Animator animator = null;
        [SerializeField] Stats stats = null;
        [SerializeField] CapsuleCollider capsuleCollider = null;

        public event System.Action OnHealthModified;

        public override void OnStartClient()
        {
            healthPoint = stats.GetMaxHealth();
        }

        public float GetHealthPoint()
        {
            return healthPoint;
        }

        public float GetHealthPercent()
        {
            return healthPoint / stats.GetMaxHealth();
        }

        public float GetMaxHealth()
        {
            return stats.GetMaxHealth();
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
            capsuleCollider.enabled = false;
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
            healthPoint = Mathf.Min(healthPoint + amount, stats.GetMaxHealth());
        }

        [Command]
        public void CmdTakeDamage(float damage)
        {
            ServerTakeDamage(damage);
        }

        private void OnHealthChanged(float oldValue, float newValue)
        {
            OnHealthModified?.Invoke();
        }

        #endregion
    }

}