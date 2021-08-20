using Dota.Controls;
using Dota.Core;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dota.Abilities
{
    public class SkillShotAbility : NetworkBehaviour, IAction
    {
        [SerializeField] GameObject indicatorPrefab = null;
        DirectionIndicator directionIndicator = null;

        [SerializeField] LayerMask groundMask = new LayerMask();
        [SerializeField] Health health = null;

        [SerializeField] GameObject skillShotPrefab = null;
        [SerializeField] Vector3 castOffset = Vector3.up * 0.5f;

        [SerializeField] NetworkAnimator networkAnimator = null;
        [SerializeField] ActionLocker actionLocker = null;

        [SerializeField] float damage = 50f;
        [SerializeField] float travelDist = 10f;

        [SerializeField] float delayTime = 1f;
        [SerializeField] float skillShotSpeed = 15f;

        #region Server
        [Server]
        public void ServerSpawnAbilityEffect(Vector3 direction)
        {
            StartCoroutine(CastSpell(direction));
        }

        [Server]
        IEnumerator CastSpell(Vector3 direction)
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 castPos = transform.position + castOffset;
            SkillShot skillShotInstance = Instantiate(skillShotPrefab, castPos, Quaternion.identity).GetComponent<SkillShot>();
            NetworkServer.Spawn(skillShotInstance.gameObject, connectionToClient);
            skillShotInstance.ServerSetDirection(castPos, direction, travelDist);
            skillShotInstance.ServerSetDamage(damage);
            skillShotInstance.ServerSetSpeed(skillShotSpeed);
            skillShotInstance.ServerSetOwner(gameObject.GetComponent<NetworkIdentity>());

            yield return null;
        }

        [Command]
        public void CmdSpawnAbilityEffect(Vector3 position)
        {
            ServerSpawnAbilityEffect(position);
        }
        #endregion


        #region Client
        public override void OnStartAuthority()
        {
            directionIndicator = Instantiate(indicatorPrefab).GetComponent<DirectionIndicator>();
            directionIndicator.SetLength(travelDist);
            directionIndicator.gameObject.SetActive(false);
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            if (health.IsDead()) { return; }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(ShowSpellUI());
            }
        }

        [Client]
        IEnumerator ShowSpellUI()
        {
            directionIndicator.gameObject.SetActive(true);

            while (true)
            {
                if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, groundMask))
                {
                    directionIndicator.SetPosition(transform.position);
                    directionIndicator.SetDirection(hit.point - transform.position);

                    if (Input.GetMouseButtonDown(0))
                    {
                        bool canDo = actionLocker.TryGetLock(this);
                        if (canDo)
                        {
                            directionIndicator.gameObject.SetActive(false);

                            networkAnimator.SetTrigger("abilityD");
                            transform.LookAt(hit.point, Vector3.up);

                            CmdSpawnAbilityEffect(directionIndicator.GetDirection());
                            break;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    directionIndicator.gameObject.SetActive(false);
                    break;
                }
                yield return null;
            }
        }

        // TODO: Make These Two Animation Events a requirement via interface or abstract class

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

        public void Stop()
        {

        }
        #endregion
    }

}

