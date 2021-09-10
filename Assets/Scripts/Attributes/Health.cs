using Mirror;
using UnityEngine;

namespace Dota.Attributes
{
    public class Health : NetworkBehaviour
    {
        [SerializeField]
        [SyncVar(hook = nameof(OnHealthChanged))]
        float healthPoint = 100f;

        [SyncVar]
        bool isDead = false;

        [SerializeField] Animator animator = null;
        [SerializeField] StatStore stats = null;
        [SerializeField] CapsuleCollider capsuleCollider = null;

        public event System.Action OnHealthModified;
        public event System.Action<Health> OnHealthDead;

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
        private void RpcNotifyHealthDead()
        {
            animator.SetTrigger("die");
            capsuleCollider.enabled = false;
            OnHealthDead?.Invoke(this);
        }

        [Server]
        public void ServerTakeDamage(float damage)
        {
            healthPoint = Mathf.Max(healthPoint - damage, 0);
            if (healthPoint == 0 && !isDead)
            {
                RpcNotifyHealthDead();
                OnHealthDead?.Invoke(this);
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

        #region Client

        // Animation Trigger Event
        public void DeathEvent()
        {

        }
        #endregion
    }

}