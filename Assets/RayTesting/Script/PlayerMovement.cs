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

    public void OpenShop()  //�}���ө�����
    {
        if (Input.GetKeyDown(KeyCode.N))        //��N
        {
            isOpen = !isOpen;
            myShop.SetActive(isOpen);    //��bool ��Panel��e��Active���A
        }
    }
 }

