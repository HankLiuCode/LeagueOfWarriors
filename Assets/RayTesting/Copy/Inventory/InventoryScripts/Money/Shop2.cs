using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop2 : MonoBehaviour
{

    public static Shop2 Instance { get; private set; }//單例,很蠢但很實用的東西,現在全世界都可以用到他了
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
        MoneyText = MoneyObj.GetComponent<Text>();//取得 MoneyObj 物件上的TEXT組件,這邊只有取到組件
        BuyButton.onClick.AddListener(OnBuyButtonDown);//對 BuyButton 按鈕做監聽的動作,只要按鈕按下去就會觸發這邊設置的方法
    }

    void OnBuyButtonDown()//當按鈕按下後會觸發的方法
    {
        if (Money > Sell)//不能=0,因為這樣0塊時他還能賒帳在買最後一次,喔...要100,因為剛剛>0但我"一次是-100",所以我可以花1塊錢賒帳99塊買到100塊的東西
        {
            Money -= Sell;
            MoneyText.text = $"${Money}";//但我們要設定他的TEXT組件底下的text屬性
        }
        else
        {
            Debug.Log("哭阿沒錢了");
        }
       
    }



    public void OnPurseSlot()
    {
        Debug.Log("購買Slot");
        ShopManager2.Instance.SelectedSlot = Slot;
    }
    public void OnPurseSlot1()
    {
        Debug.Log("購買Slot1");
        ShopManager2.Instance.SelectedSlot = Slot1;
    }
    public void OnPurseSlot2()
    {
        Debug.Log("購買Slot2");
        ShopManager2.Instance.SelectedSlot = Slot2;
    }
 }
