using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    static InventoryManager instance;

    public Inventory shop;
    public Object slotGrid;
    public Slot slotPrefab;
    public Text itemInfromation;  //拖拉 ItemDescription 的欄位

    void Awake()
    {
        if (instance != null)  //如果不等於空的
            Destroy(this);  //銷毀原本的
        instance = this;  //創建新的
    }
    private void OnEnable()
    {
        instance.itemInfromation.text = "";  //實例物件信息文本

    }
    public static void UpdateItemInfo(string itemDescription)
    {
        instance.itemInfromation.text = itemDescription;  //物件信息 = 商品描述
    }
}
