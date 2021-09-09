using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FloatingBar : MonoBehaviour
{
    public GameObject target;

    [SerializeField] Vector3 offset = Vector3.up;
    [SerializeField] Team team;
    [SerializeField] bool isAlly;

    [SerializeField] Image level = null;
    [SerializeField] Image healthBorder= null;
    [SerializeField] Image manaBorder = null;

    [SerializeField] Image healthFill = null;

    private void Update()
    {
        if(target != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position + offset);
            transform.position = screenPos;
        }

        SetBorder(team);
        SetFill(isAlly);
    }

    public void SetTarget(GameObject target, Vector3 offset)
    {
        
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

        level.color = color;
        healthBorder.color = color;
        manaBorder.color = color;
    }
}
