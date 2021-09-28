using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop2 : MonoBehaviour
{

    public static Shop2 Instance { get; private set; }//���,�������ܹ�Ϊ��F��,�{�b���@�ɳ��i�H�Ψ�L�F
    public SlotDesign Slot;
    public SlotDesign Slot1;
    public SlotDesign Slot2;

    int Money = 2000;
    public  int Sell = 0;
    public GameObject MoneyObj;
    Text MoneyText;

    public Button BuyButton;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        MoneyText = MoneyObj.GetComponent<Text>();//���o MoneyObj ����W��TEXT�ե�,�o��u������ե�
        BuyButton.onClick.AddListener(OnBuyButtonDown);//�� BuyButton ���s����ť���ʧ@,�u�n���s���U�h�N�|Ĳ�o�o��]�m����k
    }

    void OnBuyButtonDown()//����s���U��|Ĳ�o����k
    {
        if (Money > Sell)//����=0,�]���o��0���ɥL�ٯ໭�b�b�R�̫�@��,��...�n100,�]�����>0����"�@���O-100",�ҥH�ڥi�H��1�������b99���R��100�����F��
        {
            Money -= Sell;
            MoneyText.text = $"${Money}";//���ڭ̭n�]�w�L��TEXT�ե󩳤U��text�ݩ�
        }
        else
        {
            Debug.Log("�����S���F");
        }
       
    }



    public void OnPurseSlot()
    {
        Debug.Log("�ʶRSlot");
        ShopManager2.Instance.SelectedSlot = Slot;
    }
    public void OnPurseSlot1()
    {
        Debug.Log("�ʶRSlot1");
        ShopManager2.Instance.SelectedSlot = Slot1;
    }
    public void OnPurseSlot2()
    {
        Debug.Log("�ʶRSlot2");
        ShopManager2.Instance.SelectedSlot = Slot2;
    }
 }
