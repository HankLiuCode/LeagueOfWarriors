using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hpmp1 : MonoBehaviour
{
    GameObject hpbar;  ///�ŧi
    GameObject mpbar;
    // Start is called before the first frame update
    void Start()
    {
        this.hpbar = GameObject.Find("Hp");  ///�ҰʹC���������
        this.mpbar = GameObject.Find("Mp");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            this.hpbar.GetComponent<Image>().fillAmount -= 0.1f;  ///�Ѧ�����o�l�h��Image,fillmount���0.1
        }
        if (Input.GetButtonDown("Fire2"))
        {
            this.mpbar.GetComponent<Image>().fillAmount -= 0.1f;
        }



    }
}
