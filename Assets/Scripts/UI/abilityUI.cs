using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class abilityUI : MonoBehaviour
{
    [SerializeField] int index = 0;
    [SerializeField] Image cdOverlay = null;

    // Cache
    AbilityCaster abilityCaster = null;
    CooldownStore cooldownStore = null;


    private void Update()
    {
        //cdOverlay.fillAmount = cooldownStore.GetFractionRemaining(abilityCaster.GetAbility(index));
    }
}
