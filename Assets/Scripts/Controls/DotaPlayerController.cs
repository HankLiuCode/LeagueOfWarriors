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
        [SerializeField] LayerMask clickableLayer;

        bool canControl = true;

        public override void OnStartClient()
        {
            GameOverHandler.OnClientGameOver += GameOverHandler_OnClientGameOver;
        }

        private void GameOverHandler_OnClientGameOver(Base obj)
        {
            canControl = false;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            if (health.IsDead()) { return; }

            if (!canControl)
            {
                fighter.StopAttack();
                mover.End();
                return;
            }

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

        

        //[SerializeField] LayerMask attackLayer;
        //[SerializeField] LayerMask moveLayer;
        //[SerializeField] LayerMask clickableLayer;
        //[SerializeField] Actor actor = null;
        //private void Update()
        //{
        //    if (Input.GetMouseButtonDown(1))
        //    {
        //        clickableLayer.value = attackLayer.value | moveLayer.value;
        //        if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, clickableLayer))
        //        {
        //            Debug.Log("raycast Hit");
        //            int clickedObjectLayer = (1 << hit.collider.gameObject.layer);


        //            if ((clickedObjectLayer & attackLayer.value) > 0)
        //            {
        //                Debug.Log("attack");
        //                CombatTarget target = hit.collider.GetComponent<CombatTarget>();
        //                if(target != null)
        //                {
        //                    actor.Attack(target);
        //                }
        //            }
        //            else if ((clickedObjectLayer & moveLayer.value) > 0)
        //            {
        //                Debug.Log("move");
        //                actor.MoveTo(hit.point);
        //            }
        //        }
        //    }

        //    // if mouseClick on ground
        //    //    change state to moveTo clickPos
        //    // if mouseCLick on target
        //    //    change state to attack target
        //}
    }

}