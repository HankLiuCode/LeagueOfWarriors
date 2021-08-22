﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;
using Dota.Combat;
using Dota.Movement;

namespace Dota.Controls
{
    public class DotaPlayerController : NetworkBehaviour
    {
        [SerializeField] DotaMover mover = null;
        [SerializeField] DotaFighter fighter = null;
        [SerializeField] Health health = null;

        private void Update()
        {
            if (!hasAuthority) { return; }

            if (health.IsDead()) { return; }

            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(GetMouseRay(), out RaycastHit hit, Mathf.Infinity))
                {
                    GameObject go = hit.collider.gameObject;
                    if(go == gameObject) { return; }

                    if (fighter.CanAttack(go))
                    {
                        fighter.StartAttack(go);
                        return;
                    }
                    else
                    {
                        fighter.StopAttack();
                        Debug.Log(hit.collider.name);
                        mover.MoveTo(hit.point);
                    }
                }
            }

        }



        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

}