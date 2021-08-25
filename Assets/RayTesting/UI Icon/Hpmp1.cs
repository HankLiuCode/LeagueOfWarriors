using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hpmp1 : MonoBehaviour
{
    GameObject hpbar;  ///宣告
    GameObject mpbar;
    // Start is called before the first frame update
    void Start()
    {
        this.hpbar = GameObject.Find("Hp");  ///啟動遊戲血條物件
        this.mpbar = GameObject.Find("Mp");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            this.hpbar.GetComponent<Image>().fillAmount -= 0.1f;  ///由血條取得子層級Image,fillmount減少0.1
        }
        if (Input.GetButtonDown("Fire2"))
        {
            this.mpbar.GetComponent<Image>().fillAmount -= 0.1f;
        }



    }
}
