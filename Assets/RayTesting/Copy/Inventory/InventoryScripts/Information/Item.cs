using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Inventory/New Item")]  //�i�b��Ƨ��̫��k��Create-Inventory-NewItem�s�W�i�������~�ƭȪŪ���
public class Item : ScriptableObject
{
    public string itemName;  //����W

    public int PhysicalAttack;
    public int ManaAttack;

    public int PhysicalDefence;
    public int ManaDefence;

    public int Health;
    public int Mana;

    public int speed;

    [TextArea]
    public string itemInfo;  //���󤶲�
    public bool equip;  //�O�_�i�˳�
}
