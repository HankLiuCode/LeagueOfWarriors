using Dota.UI;
using Dota.Attributes;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField] GameObject infoPanel = null;
    [SerializeField] IconDisplay iconDisplay = null;
    [SerializeField] HealthDisplay healthDisplay = null;
    [SerializeField] ManaDisplay manaDisplay = null;
    [SerializeField] StatsDisplay statsDisplay = null;

    public void SetInfo(StatStore stats)
    {
        IIconOwner iconOwner = stats.GetComponent<IIconOwner>();
        Health health = stats.GetComponent<Health>();
        Mana mana = stats.GetComponent<Mana>();

        iconDisplay.SetIcon(iconOwner.GetIcon());
        healthDisplay.SetHealth(health);
        manaDisplay.SetMana(mana);
        statsDisplay.SetStats(stats);
    }

    private void Start()
    {
        infoPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity))
            {
                GameObject go = hit.collider.gameObject;
                StatStore stats = go.GetComponent<StatStore>();

                if (stats != null)
                {
                    infoPanel.SetActive(true);
                    SetInfo(stats);
                }
                else
                {
                    infoPanel.SetActive(false);
                }
            }
        }
    }
}
