using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeamMember
{
    public Team GetTeam();
    public void SetTeam(Team team);
}
