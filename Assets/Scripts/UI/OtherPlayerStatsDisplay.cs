using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Controls;
using Dota.UI;
using Dota.Core;

public class OtherPlayerStatsDisplay : MonoBehaviour
{
    public List<GameObject> playerDisplays;
    
    public void BindPlayersToDisplays(List<DotaPlayerController> dotaPlayerControllers)
    {
        for(int i=0; i<dotaPlayerControllers.Count; i++)
        {
            Health health = dotaPlayerControllers[i].GetComponent<Health>();
            Mana mana = dotaPlayerControllers[i].GetComponent<Mana>();

            playerDisplays[i].GetComponent<HealthDisplay>().SetHealth(health);
            playerDisplays[i].GetComponent<ManaDisplay>().SetMana(mana);
        }
    }
}
