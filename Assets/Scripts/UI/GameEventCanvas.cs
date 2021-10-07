using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventCanvas : MonoBehaviour
{
    private void Awake()
    {
        Champion.ClientOnChampionDeadAttacker += Champion_ClientOnChampionDeadAttacker;
    }

    private void OnDestroy()
    {
        Champion.ClientOnChampionDeadAttacker -= Champion_ClientOnChampionDeadAttacker;
    }

    private void Champion_ClientOnChampionDeadAttacker(Champion arg1, NetworkIdentity arg2)
    {
        Debug.Log(arg1.name + "has been slain by" + arg2.name);
    }
}
