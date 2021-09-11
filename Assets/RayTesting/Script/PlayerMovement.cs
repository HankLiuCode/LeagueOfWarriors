using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public GameObject myShop;
    bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        myShop.SetActive(false);
        Debug.Log("111");
    }

    void Update()
    {
        OpenShop();
    }

    public void OpenShop()  //開關商店介面
    {
        if (Input.GetKeyDown(KeyCode.N))        //按N
        {
            isOpen = !isOpen;
            myShop.SetActive(isOpen);    //用bool 改Panel當前的Active狀態
        }
    }
 }

