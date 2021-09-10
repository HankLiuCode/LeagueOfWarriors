using Dota.Controls;
using Dota.Core;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dota.Abilities
{
    public class SkillShotAbility : Ability
    {
        [SerializeField] GameObject indicatorPrefab = null;
        RectIndicator directionIndicator = null;

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
            directionIndicator = Instantiate(indicatorPrefab).GetComponent<RectIndicator>();
            directionIndicator.SetLength(travelDist);
            directionIndicator.gameObject.SetActive(false);
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

        public override void ShowIndicator()
        {
            directionIndicator.gameObject.SetActive(true);
        }

        public override void UpdateIndicator(AbilityData abilityData)
        {
            directionIndicator.SetPosition(abilityData.casterPos);
            directionIndicator.SetDirection(abilityData.mousePos - abilityData.casterPos);
        }

        public override void HideIndicator()
        {
            directionIndicator.gameObject.SetActive(false);
        }

        public override void Cast(AbilityData abilityData)
        {
            bool canDo = actionLocker.TryGetLock(this);
            if (canDo)
            {
                networkAnimator.SetTrigger("abilityD");

                transform.LookAt(abilityData.mousePos, Vector3.up);

                abilityData.delayTime = delayTime;

                Vector3 direction = (abilityData.mousePos - abilityData.casterPos).normalized;
                CmdSpawnAbilityEffect(direction);
            }
        }
        #endregion
    }

}

