using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory",menuName = "Inventory/New Inventory")]  ////�i�b��Ƨ��̫��k��Create-Inventory-New Inventory�s�W�i�������~�ƭȪŪ���
public class Inventory : ScriptableObject
{
    public List<Item> itemList = new List<Item>();

    [TextArea]
    public string interfaceInfo;  //����H��
}
