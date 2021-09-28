using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus2 : MonoBehaviour
{
    public int StartMoney = 2000; // 初始金幣
    public static int Money; //當前剩餘多少的錢
    private void Start()
    {
        Money = StartMoney;
    }
}

