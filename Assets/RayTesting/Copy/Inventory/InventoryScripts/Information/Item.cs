using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Inventory/New Item")]  //可在資料夾裡按右鍵Create-Inventory-NewItem新增可控的物品數值空物件
public class Item : ScriptableObject
{
    public string itemName;  //物件名

    public int PhysicalAttack;
    public int ManaAttack;

    public int PhysicalDefence;
    public int ManaDefence;

    public int Health;
    public int Mana;

    public int speed;

    [TextArea]
    public string itemInfo;  //物件介紹
    public bool equip;  //是否可裝備
}
