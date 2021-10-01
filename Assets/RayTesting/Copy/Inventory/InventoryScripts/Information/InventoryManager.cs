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
    public Text itemInfromation;  //��� ItemDescription �����

    void Awake()
    {
        if (instance != null)  //�p�G������Ū�
            Destroy(this);  //�P���쥻��
        instance = this;  //�Ыطs��
    }
    private void OnEnable()
    {
        instance.itemInfromation.text = "";  //��Ҫ���H���奻

    }
    public static void UpdateItemInfo(string itemDescription)
    {
        instance.itemInfromation.text = itemDescription;  //����H�� = �ӫ~�y�z
    }
}
