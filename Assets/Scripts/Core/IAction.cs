using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Core
{
    public interface IAction
    {
        public int GetPriority();
        public void Stop();
    }
}