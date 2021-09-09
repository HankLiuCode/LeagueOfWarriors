using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsRow : MonoBehaviour
{
    [SerializeField] Image rowIcon;
    [SerializeField] TextMeshProUGUI rowValue;
    [SerializeField] StringIconMapping stringIconMapping;

    public void SetRow(string name, float value)
    {
        Debug.Log("SetRow");
        rowIcon.sprite = stringIconMapping.GetIcon(name);
        rowValue.text = value.ToString("F1");
    }
}
