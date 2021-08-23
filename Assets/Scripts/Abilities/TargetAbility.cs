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
    public class TargetAbility : NetworkBehaviour, IAction, IAbility
    {
        [SerializeField] GameObject spellRangePrefab = null;
        AreaIndicator spellRangeInstance = null;

        [SerializeField] LayerMask playerMask = new LayerMask();
        [SerializeField] Health health = null;

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

        [Command]
        public void CmdSpawnProjectile(Health health)
        {
            ServerSpawnProjectile(health);
        }
        #endregion


        #region Client

        public override void OnStartAuthority()
        {
            spellRangeInstance = Instantiate(spellRangePrefab).GetComponent<AreaIndicator>();
            spellRangeInstance.SetRadius(maxRange);
            spellRangeInstance.gameObject.SetActive(false);
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            if (health.IsDead()) { return; }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(ShowSpellUI());
            }
        }

        [Client]
        IEnumerator ShowSpellUI()
        {
            spellRangeInstance.gameObject.SetActive(true);

            while (true)
            {
                spellRangeInstance.SetPosition(transform.position);

                if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, playerMask))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        spellRangeInstance.gameObject.SetActive(false);

                        Health health = hit.collider.GetComponent<Health>();
                        if (health && !health.IsDead())
                        {
                            Vector3 targetPos = health.transform.position;

                            if (Vector3.Distance(targetPos, transform.position) < maxRange)
                            {
                                bool canDo = actionLocker.TryGetLock(this);
                                if (canDo)
                                {
                                    networkAnimator.SetTrigger("abilityD");
                                    transform.LookAt(hit.point, Vector3.up);
                                    CmdSpawnProjectile(health);
                                }
                            }
                        }
                        break;
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    spellRangeInstance.gameObject.SetActive(false);
                    break;
                }
                yield return null;
            }
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

        public int GetPriority()
        {
            return 1;
        }

        public void End()
        {

        }

        public void Begin()
        {

        }
        #endregion
    }
}
