using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop2 : MonoBehaviour
{

    public static Shop2 Instance { get; private set; }//單例,很蠢但很實用的東西,現在全世界都可以用到他了

    int Money = 3500;  //初始金額
    public int Sell = 0;
    public GameObject MoneyObj;  //用來拖放MoneyGameObject
    Text MoneyText;
    public Button BuyButton;  //拖放Button按鍵
    


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        MoneyText = MoneyObj.GetComponent<Text>();//取得 MoneyObj 物件上的TEXT組件,這邊只有取到組件
        BuyButton.onClick.AddListener(OnBuyButtonDown);//對 BuyButton 按鈕做監聽的動作,只要按鈕按下去就會觸發這邊設置的方法
    }

    public void OnBuyButtonDown()//當按鈕按下後會觸發的方法
    {
        if (Money > Sell)
        {
            Money -= Sell;
            MoneyText.text = $"${Money}";//但我們要設定他的TEXT組件底下的text屬性
        }
        else
        {
            Debug.Log("哭阿沒錢了");
        }
    }
}
