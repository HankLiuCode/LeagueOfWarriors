using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int StartMoney = 800; // ��l����
    public static int Money; //��e�Ѿl�h�֪���
    private void Start()
    {
        Money = StartMoney;
    }
}
