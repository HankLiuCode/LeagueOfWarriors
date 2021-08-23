using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Core
{
    public class ActionLocker : MonoBehaviour
    {
        IAction turn;
        bool isLocked = false;

        public bool TryGetLock(IAction action)
        {
            if (turn == action) { return true; }

            if(turn == null)
            {
                turn = action;
                turn.Begin();
                isLocked = true;
                return true;
            }
            else if(turn.GetPriority() < action.GetPriority())
            {
                turn.End();
                turn = action;
                turn.Begin();
                isLocked = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ReleaseLock(IAction owner)
        {
            if(owner == turn)
            {
                turn.End();
                turn = null;
                isLocked = false;
            }
        }
    }

}