using Dota.Controls;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] int index = -1;
    [SerializeField] Image abilityIcon = null;
    [SerializeField] Image cdOverlay = null;
    [SerializeField] Image unreadyOverlay = null;
    [SerializeField] TextMeshProUGUI cdTime = null;

    // Cache
    AbilityCaster abilityCaster = null;
    CooldownStore cooldownStore = null;
    float currentFillAmount = 0;

    public void SetUp(AbilityCaster abilityCaster)
    {
        this.abilityCaster = abilityCaster;
        cooldownStore = abilityCaster.GetComponent<CooldownStore>();
        abilityIcon.sprite = abilityCaster.GetAbility(index).GetIcon();
    }

    private void Start()
    {
        unreadyOverlay.gameObject.SetActive(false);
        cdTime.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(abilityCaster != null)
        {
            Ability ability = abilityCaster.GetAbility(index);
            float targetFillAmount = cooldownStore.GetFractionRemaining(ability);
            currentFillAmount = targetFillAmount;

            float timeRemaining = cooldownStore.GetTimeRemaining(ability);

            cdOverlay.fillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, 0.1f);
            cdTime.text = timeRemaining.ToString("F0");

            if(timeRemaining <= 0)
            {
                unreadyOverlay.gameObject.SetActive(false);
                cdTime.gameObject.SetActive(false);
            }
            else
            {
                unreadyOverlay.gameObject.SetActive(true);
                cdTime.gameObject.SetActive(true);
            }
        }
    }
}
