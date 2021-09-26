﻿using System.Collections;
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
        [SerializeField] LayerMask clickableLayer;

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            if (health.IsDead()) { return; }

            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, clickableLayer))
                {
                    GameObject go = hit.collider.gameObject;

                    if (fighter.IsAttackable(go))
                    {
                        if (go == gameObject) { return; }

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