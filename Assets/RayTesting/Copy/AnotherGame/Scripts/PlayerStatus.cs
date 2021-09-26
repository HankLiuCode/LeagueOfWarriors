using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int StartMoney = 800; // 初始金幣
    public static int Money; //當前剩餘多少的錢
    private void Start()
    {
        Money = StartMoney;
    }
}
