using Dota.Combat;
using Dota.Controls;
using Dota.Core;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Abilities
{
    // TODO: Need to add who is the caster
    public class TargetAbility : Ability
    {
        [SerializeField] GameObject spellRangePrefab = null;
        CircleIndicator spellRangeInstance = null;

        [SerializeField] GameObject targetProjectilePrefab = null;
        [SerializeField] Vector3 castOffset = Vector3.up * 0.5f;

        [SerializeField] NetworkAnimator networkAnimator = null;
        [SerializeField] ActionLocker actionLocker = null;

        [SerializeField] float damage = 50f;
        [SerializeField] float maxRange = 5f;

        [SerializeField] float delayTime = 0.1f;

        #region Server
        [Server]
        public void ServerSpawnProjectile(Health target)
        {
            StartCoroutine(CastSpell(target));
        }

        [Command]
        public void CmdSpawnProjectile(Health health)
        {
            ServerSpawnProjectile(health);
        }

        [Server]
        IEnumerator CastSpell(Health health)
        {
            yield return new WaitForSeconds(delayTime);

            Vector3 castPos = transform.position + castOffset;
            DotaProjectile projectile = Instantiate(targetProjectilePrefab, castPos, Quaternion.identity).GetComponent<DotaProjectile>();

            NetworkServer.Spawn(projectile.gameObject);
            projectile.ServerSetOwner(gameObject);
            projectile.SetTarget(health, damage);

            yield return null;
        }
        #endregion


        #region Client

        public override void OnStartAuthority()
        {
            spellRangeInstance = Instantiate(spellRangePrefab).GetComponent<CircleIndicator>();
            spellRangeInstance.SetRadius(maxRange);
            spellRangeInstance.gameObject.SetActive(false);
        }

        // Animation Event
        private void AttackPoint()
        {

        }

        // Animation Event
        private void AttackBackSwing()
        {
            actionLocker.ReleaseLock(this);
        }

        public override void ShowIndicator()
        {
            spellRangeInstance.gameObject.SetActive(true);
        }

        public override void UpdateIndicator(AbilityData abilityData)
        {
            spellRangeInstance.SetPosition(abilityData.casterPos);
        }

        public override void HideIndicator()
        {
            spellRangeInstance.gameObject.SetActive(false);
        }

        public override void Cast(AbilityData abilityData)
        {
            if (abilityData.target == null) { return; }

            Health health = abilityData.target.GetComponent<Health>();

            if (!health) { return; }

            if (Vector3.Distance(health.transform.position, abilityData.casterPos) > maxRange) { return; }


            bool canDo = actionLocker.TryGetLock(this);
            if (canDo)
            {
                networkAnimator.SetTrigger("abilityD");

                transform.LookAt(abilityData.mouseClickPos, Vector3.up);

                abilityData.delayTime = delayTime;

                CmdSpawnProjectile(health);
            }
        }
        #endregion
    }
}
