using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Controls;
using Dota.UI;
using Dota.Core;

public class OtherStatsDisplay : MonoBehaviour
{
    public List<GameObject> playerDisplays;
    public List<DotaPlayerController> players;



    public void AddPlayer(DotaPlayerController dotaPlayerController)
    {
        players.Add(dotaPlayerController);

        for(int i=0; i<players.Count; i++)
        {
            Health health = players[i].GetComponent<Health>();
            Mana mana = players[i].GetComponent<Mana>();

            playerDisplays[i].GetComponent<HealthDisplay>().SetHealth(health);
            playerDisplays[i].GetComponent<ManaDisplay>().SetMana(mana);
        }
    }
}
