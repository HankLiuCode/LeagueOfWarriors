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

    [SerializeField] LayerMask displayLayer;

    public void SetInfo(StatStore stats)
    {
        IIconOwner iconOwner = stats.GetComponent<IIconOwner>();

        if(iconOwner == null) { Debug.LogError(stats.name + "Does not implement IIconOwner"); }

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
            bool showInfo = false;
            if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, displayLayer))
            {
                GameObject go = hit.collider.gameObject;
                StatStore stats = go.GetComponent<StatStore>();

                if (stats != null)
                {
                    showInfo = true;
                    SetInfo(stats);
                }
                else
                {
                    showInfo = false;
                }
            }
            infoPanel.SetActive(showInfo);
        }
    }
}
