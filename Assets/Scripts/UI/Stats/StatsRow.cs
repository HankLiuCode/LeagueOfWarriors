using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsRow : MonoBehaviour
{
    [SerializeField] Image rowIcon;
    [SerializeField] TextMeshProUGUI rowValue;

    public void SetRow(Sprite icon, float value)
    {
        rowIcon.sprite = icon;
        rowValue.text = value.ToString("F1");
    }
}
