using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item slotItem;
    //public Image slotImage;
    //public Text slotNum;

    //�W���O�쥻�]��

    public int Sell;  //���~���

   public void ItemOnClicked()  //��ƹ��I���o�Ӫ��~��
    {
        Shop2.Instance.Sell = Sell;  //Ssop2���䪺����|����o�䪺���

        InventoryManager.UpdateItemInfo(slotItem.itemInfo);  //��sInventoryManager�̭���itemInfo

    }
}
