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

        [SerializeField] float deadExp = 50f;

        public event System.Action<float, float> ClientOnHealthModified;
        public event System.Action<Health, NetworkIdentity> ClientOnHealthDead;
        public event System.Action ClientOnHealthDeadEnd;

        public event System.Action ServerOnHealthModified;
        public event System.Action<Health, NetworkIdentity> ServerOnHealthDead;
        public event System.Action ServerOnHealthDeadEnd;
        

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
        public void ServerTakeDamage(float damage, NetworkIdentity attacker)
        {
            healthPoint = Mathf.Max(healthPoint - damage, 0);
            if (healthPoint == 0 && !isDead)
            {
                RpcNotifyHealthDead(attacker);
                ServerOnHealthDead?.Invoke(this, attacker);
                isDead = true;

                IRewardReceiver receiver = attacker.GetComponent<IRewardReceiver>();
                receiver.SendExp(deadExp);
            }
        }

        //[Server]
        //public void ServerTakeDamage(float damage)
        //{
        //    healthPoint = Mathf.Max(healthPoint - damage, 0);
        //    if (healthPoint == 0 && !isDead)
        //    {
        //        RpcNotifyHealthDead();
        //        ServerOnHealthDead?.Invoke(this);
        //        isDead = true;
        //    }
        //}

        [Server]
        public void ServerHeal(float amount)
        {
            if(isDead) { return; }

            healthPoint = Mathf.Min(healthPoint + amount, GetMaxHealth());
        }

        [Command]
        public void CmdTakeDamage(float damage, NetworkIdentity attacker)
        {
            ServerTakeDamage(damage, attacker);
        }

        [Command]
        public void CmdHeal(float amount)
        {
            ServerHeal(amount);
        }

        private void OnHealthChanged(float oldValue, float newValue)
        {
            ClientOnHealthModified?.Invoke(oldValue, newValue);
        }
        #endregion

        #region Client

        [ClientRpc]
        private void RpcNotifyHealthDead(NetworkIdentity attacker)
        {
            animator.SetTrigger("die");
            healthCollider.enabled = false;
            ClientOnHealthDead?.Invoke(this, attacker);
        }
        
        // Animation Trigger Event
        private void AnimationEventHandler_OnDeathEnd()
        {
            ClientOnHealthDeadEnd?.Invoke();
            ServerOnHealthDeadEnd?.Invoke();
        }
        #endregion
    }

}