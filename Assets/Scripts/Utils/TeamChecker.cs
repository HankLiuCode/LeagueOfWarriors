using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Utils
{
    public static class TeamChecker
    {
        public static bool IsSameTeam(GameObject go1, GameObject go2)
        {
            ITeamMember go1TeamMem = go1.GetComponent<ITeamMember>();
            ITeamMember go2TeamMem = go2.GetComponent<ITeamMember>();

            if(go1TeamMem == null || go2TeamMem == null) { return false; }

            Team go1Team = go1TeamMem.GetTeam();
            Team go2Team = go2TeamMem.GetTeam();

            if(go1Team != go2Team) { return false; }

            return true;

        }
    }
}
