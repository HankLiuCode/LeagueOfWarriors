using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; ///呼叫ai引擎
public class Navi1 : MonoBehaviour
{
    public Transform Target1; ///宣告目標，後面的Target1可自創名稱
    NavMeshAgent Agent1; ///宣告導航代理器
    // Start is called before the first frame update
    void Start()
    {
        Agent1 = GetComponent<NavMeshAgent>(); ///啟動導航
    }

    // Update is called once per frame
    void Update()
    { 
       Agent1.SetDestination(Target1.position); ///設定導航目標
    }
}
