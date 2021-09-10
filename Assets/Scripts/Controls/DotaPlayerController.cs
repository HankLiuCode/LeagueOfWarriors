using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Attributes;
using Dota.Combat;
using Dota.Movement;

namespace Dota.Controls
{
    public class DotaPlayerController : NetworkBehaviour
    {
        [SerializeField] DotaMover mover = null;
        [SerializeField] DotaFighter fighter = null;
        [SerializeField] Health health = null;

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) 
            {
                return; 
            }

            if (health.IsDead()) { return; }

            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity))
                {
                    GameObject go = hit.collider.gameObject;
                    if (go == gameObject) { return; }

                    if (fighter.IsAttackable(go))
                    {
                        fighter.StartAttack(go);
                        return;
                    }
                    else
                    {
                        fighter.StopAttack();
                        mover.MoveTo(hit.point);
                    }
                }
            }
        }
    }

}