using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "My Ability", menuName = "Abilities/New Ability")]
public class Ability : ScriptableObject
{
    [SerializeField] GameObject indicator;
}
