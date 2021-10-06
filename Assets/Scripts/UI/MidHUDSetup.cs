using UnityEngine;
using Dota.UI;
using Dota.Attributes;
using Mirror;

public class MidHUDSetup : MonoBehaviour
{
    [SerializeField] IconDisplay iconDisplay ;
    [SerializeField] HealthDisplay healthDisplay ;
    [SerializeField] ManaDisplay manaDisplay;
    [SerializeField] GameObject abilitiesContainer ;

    private void Start()
    {
        Champion.OnChampionSpawned += Champion_OnChampionSpawned;
    }

    private void OnDestroy()
    {
        Champion.OnChampionSpawned -= Champion_OnChampionSpawned;
    }

    private void Champion_OnChampionSpawned(Champion champion)
    {
        if (champion.hasAuthority)
        {
            Health health = champion.GetComponent<Health>();
            Mana mana = champion.GetComponent<Mana>();

            iconDisplay.SetIcon(champion.GetIcon());
            healthDisplay.SetHealth(health);
            manaDisplay.SetMana(mana);

            AbilityUI[] abilityUIs = abilitiesContainer.GetComponentsInChildren<AbilityUI>();
            foreach (AbilityUI abilityUI in abilityUIs)
            {
                abilityUI.SetUp(champion.GetComponent<AbilityCaster>());
            }
        }
    }
}
