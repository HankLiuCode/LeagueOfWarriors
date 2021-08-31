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

    public void SetUp(DotaPlayerController dotaPlayerController)
    {
        abilityCaster = dotaPlayerController.GetComponent<AbilityCaster>();
        cooldownStore = dotaPlayerController.GetComponent<CooldownStore>();
    }

    private void Update()
    {
        cdOverlay.fillAmount = cooldownStore.GetFractionRemaining(abilityCaster.GetAbility(index));
    }
}
