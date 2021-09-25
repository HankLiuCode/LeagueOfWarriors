using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringIconMapping")]
public class StringIconMapping : ScriptableObject
{
    public List<StringIcon> stringIcons;

    public Sprite GetIcon(string iconName)
    {
        foreach(StringIcon si in stringIcons)
        {
            if(si.iconName == iconName)
            {
                return si.sprite;
            }
        }
        return null;
    }
}

[System.Serializable]
public struct StringIcon
{
    public string iconName;
    public Sprite sprite;
}