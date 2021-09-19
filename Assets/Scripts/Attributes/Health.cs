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
        [SerializeField] Collider healthCollider = null;
        [SerializeField] AnimationEventHandler animationEventHandler = null;
        [SerializeField] float healthDisplayOffset = 3f;
        
        public event System.Action OnHealthRevive;
        public event System.Action OnHealthModified;
        public event System.Action<Health> OnHealthDead;
        public event System.Action OnHealthDeadEnd;

        public override void OnStartClient()
        {
            healthPoint = stats.GetStats().maxHealth;
            animationEventHandler.OnDeathEnd += AnimationEventHandler_OnDeathEnd;
        }

        public float GetDisplayOffset()
        {
            return healthDisplayOffset;
        }

        public float GetHealthPoint()
        {
            return healthPoint;
        }

        public float GetHealthPercent()
        {
            return healthPoint / stats.GetStats().maxHealth;
        }

        public float GetMaxHealth()
        {
            return stats.GetStats().maxHealth;
        }
        
        public bool IsDead()
        {
            return isDead;
        }

        #region Server

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
        public void ServerRevive()
        {
            if (isDead)
            {
                isDead = false;
                ServerHeal(GetMaxHealth());
                RpcNotifyHealthRevive();
                OnHealthRevive?.Invoke();
            }
        }

        [Server]
        public void ServerHeal(float amount)
        {
            if(isDead) { return; }

            healthPoint = Mathf.Min(healthPoint + amount, GetMaxHealth());
        }

        [Command]
        public void CmdTakeDamage(float damage)
        {
            ServerTakeDamage(damage);
        }

        [Command]
        public void CmdHeal(float amount)
        {
            ServerHeal(amount);
        }

        private void OnHealthChanged(float oldValue, float newValue)
        {
            OnHealthModified?.Invoke();
        }
        #endregion

        #region Client

        [ClientRpc]
        private void RpcNotifyHealthDead()
        {
            Debug.Log("NotifyHealthDead");
            animator.ResetTrigger("revive");
            animator.SetTrigger("die");
            healthCollider.enabled = false;
            OnHealthDead?.Invoke(this);
        }

        [ClientRpc]
        private void RpcNotifyHealthRevive()
        {
            Debug.Log("NotifyHealthRevive");
            animator.SetTrigger("revive");
            healthCollider.enabled = true;
            OnHealthRevive?.Invoke();
        }
        
        // Animation Trigger Event
        private void AnimationEventHandler_OnDeathEnd()
        {
            // do disappear shader
            OnHealthDeadEnd?.Invoke();
        }
        #endregion
    }

}