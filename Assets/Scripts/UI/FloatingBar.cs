using Dota.Attributes;
using Dota.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class FloatingBar : MonoBehaviour
{
    public GameObject target;

    [SerializeField] Vector3 offset = Vector3.up;
    [SerializeField] Team team;
    [SerializeField] bool isAlly;

    [SerializeField] HealthDisplay healthDisplay = null;
    [SerializeField] ManaDisplay manaDisplay = null;

    Level level;
    [SerializeField] Image levelImage = null;
    [SerializeField] TextMeshProUGUI levelText = null;



    [SerializeField] Image healthBorder= null;
    [SerializeField] Image manaBorder = null;
    [SerializeField] Image healthFill = null;

    private void Update()
    {
        if(target != null)
        {
            UpdateFloatingBarPosition(target.transform.position);
            if(level != null)
            {
                levelText.text = level.GetLevel().ToString();
            }
        }
    }

    private void UpdateFloatingBarPosition(Vector3 followPoint)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(followPoint + offset);
        transform.position = screenPos;
    }

    public void SetLevel(Level level)
    {
        this.level = level;
        this.levelImage.gameObject.SetActive(true);
    }

    public void Setup(Health health, Mana mana, Team localPlayerTeam, Team targetTeam, Vector3 offset)
    {
        healthDisplay.SetHealth(health);
        manaDisplay.SetMana(mana);
        bool isAlly = (localPlayerTeam == targetTeam);

        SetBorder(targetTeam);
        SetFill(isAlly);

        UpdateFloatingBarPosition(health.transform.position);

        this.team = targetTeam;
        this.isAlly = isAlly;
        this.target = health.gameObject;
        this.offset = offset;
    }

    public void SetFill(bool isAlly)
    {
        healthFill.color = isAlly ? Color.green : Color.red;
    }

    public void SetBorder(Team team)
    {
        Color color = Color.black;
        switch (team)
        {
            case Team.Red:
                color = Color.red;
                break;

            case Team.Blue:
                color = Color.blue;
                break;
        }

        levelImage.color = color;
        healthBorder.color = color;
        manaBorder.color = color;
    }
}
