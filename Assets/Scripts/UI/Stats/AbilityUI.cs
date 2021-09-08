using Dota.Controls;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] int index = 0;
    [SerializeField] Image cdOverlay = null;

    // Cache
    AbilityCaster abilityCaster = null;
    CooldownStore cooldownStore = null;
    float currentFillAmount = 0;

    public void SetUp(AbilityCaster abilityCaster)
    {
        this.abilityCaster = abilityCaster;
        cooldownStore = abilityCaster.GetComponent<CooldownStore>();
    }

    private void Update()
    {
        Ability ability = abilityCaster.GetAbility(index);
        float targetFillAmount = cooldownStore.GetFractionRemaining(ability);
        currentFillAmount = targetFillAmount;
        cdOverlay.fillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, 0.1f);
    }
}
