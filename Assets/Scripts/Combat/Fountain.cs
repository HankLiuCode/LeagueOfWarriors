using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Attributes;

public class Fountain : NetworkBehaviour
{
    [SerializeField] float healPerSecond = 50f;
    [SerializeField] float manaPerSecond = 50f;


    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if(health != null)
        {
            health.ServerHeal(healPerSecond * Time.deltaTime);
        }

        Mana mana = other.GetComponent<Mana>();
        if(mana != null)
        {
            mana.ServerRegenMana(manaPerSecond * Time.deltaTime);
        }
    }
}
