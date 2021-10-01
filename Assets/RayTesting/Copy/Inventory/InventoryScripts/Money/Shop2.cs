using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop2 : MonoBehaviour
{

    public static Shop2 Instance { get; private set; }//���,�������ܹ�Ϊ��F��,�{�b���@�ɳ��i�H�Ψ�L�F

    int Money = 3500;  //��l���B
    public int Sell = 0;
    public GameObject MoneyObj;  //�Ψө��MoneyGameObject
    Text MoneyText;
    public Button BuyButton;  //���Button����
    


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        MoneyText = MoneyObj.GetComponent<Text>();//���o MoneyObj ����W��TEXT�ե�,�o��u������ե�
        BuyButton.onClick.AddListener(OnBuyButtonDown);//�� BuyButton ���s����ť���ʧ@,�u�n���s���U�h�N�|Ĳ�o�o��]�m����k
    }

    public void OnBuyButtonDown()//����s���U��|Ĳ�o����k
    {
        if (Money > Sell)
        {
            Money -= Sell;
            MoneyText.text = $"${Money}";//���ڭ̭n�]�w�L��TEXT�ե󩳤U��text�ݩ�
        }
        else
        {
            Debug.Log("�����S���F");
        }
    }
}
