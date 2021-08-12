using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStore : MonoBehaviour
{
    [SerializeField] Ability[] abilities = new Ability[4];
    
    public void Use(int index, GameObject user)
    {
        abilities[index].Use(user);
    }
}
