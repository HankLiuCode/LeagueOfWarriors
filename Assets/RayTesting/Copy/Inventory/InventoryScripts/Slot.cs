using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item slotItem;
    public Image slotImage;
    public Text slotNum;

    //�W���O�A�ۤv��
    public int Sell;//���~���

   public void ItemOnClicked()//��ƹ��I���o�Ӫ��~��
    {
        Shop2.Instance.Sell = Sell;//�ө����䪺����|����ۤv�����
        InventoryManager.UpdateItemInfo(slotItem.itemInfo);//�o�����D�F��,�A�g��
    }
}
