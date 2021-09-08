using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rowName;
    [SerializeField] TextMeshProUGUI rowValue;

    public void SetRow(string name, float value)
    {
        rowName.text = name;
        rowValue.text = value.ToString("F1");
    }
}
