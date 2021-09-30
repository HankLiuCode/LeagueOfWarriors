using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory",menuName = "Inventory/New Inventory")]  ////可在資料夾裡按右鍵Create-Inventory-New Inventory新增可控的物品數值空物件
public class Inventory : ScriptableObject
{
    public List<Item> itemList = new List<Item>();

    [TextArea]
    public string interfaceInfo;  //物件信息
}
