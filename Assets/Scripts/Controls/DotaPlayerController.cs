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
        [SerializeField] ClientMover mover = null;
        [SerializeField] ClientFighter fighter = null;
        [SerializeField] Health health = null;

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            if (health.IsDead()) { return; }

            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity))
                {
                    GameObject go = hit.collider.gameObject;

                    if (fighter.IsAttackable(go))
                    {
                        Debug.Log("Is Attackable");
                        if (go == gameObject) { return; }

                        fighter.StartAttack(go);
                        return;
                    }
                    else
                    {
                        Debug.Log("Is Not Attackable");
                        fighter.StopAttack();
                        mover.MoveTo(hit.point);
                    }
                }
            }
        }
    }

}