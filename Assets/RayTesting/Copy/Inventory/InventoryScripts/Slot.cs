using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item slotItem;
    public Image slotImage;
    public Text slotNum;

    //上面是你自己的
    public int Sell;//物品售價

   public void ItemOnClicked()//當滑鼠點擊這個物品時
    {
        Shop2.Instance.Sell = Sell;//商店那邊的售價會等於自己的售價
        InventoryManager.UpdateItemInfo(slotItem.itemInfo);//這不知道幹嘛,你寫的
    }
}
