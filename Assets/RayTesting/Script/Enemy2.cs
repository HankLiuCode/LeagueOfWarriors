using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float t1; ///定義幾秒一隻怪物
    private float t2; ///內部定義時間
    // Start is called before the first frame update
    void Start()
    {
        t2 = t1; ///一開始，內部定義時間=間隔時間
    }

    // Update is called once per frame
    void Update()
    {
        t2 = t2 - Time.deltaTime; ///Time.deltaTime計時工具
        Debug.Log(t2);

        if (t2 <= 0) ///如果內部時間小於0，就恢復成間隔時間
        {
            t2 = t1;
        }
    }
}
