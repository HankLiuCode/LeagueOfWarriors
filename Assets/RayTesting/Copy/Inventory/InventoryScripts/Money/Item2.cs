using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item2 : MonoBehaviour
{
    //public GameObject PlayerStatus2;
    //PlayerStatus2 PlayS;

    private Color hoverColor = Color.gray;
    private Color initColor;
    private Renderer render;
    void Start()
    {
        //PlayerStatus2.GetComponent<PlayerStatus2>();

        //render = GetComponent<MeshRenderer>();
        //initColor = render.material.color;
    }
    private void Awake()
    {
        //PlayS = PlayerStatus2.GetComponent<PlayerStatus2>();
    }


    private void OnMouseEnter()
    {
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        //if (ShopManager2.Instance.SelectedSlot == null) return;  //�P�_�p�G�Onull,�᭱�N����ާ@
        //render.material.color = hoverColor;  // ���в��W�����C��
    }
    private void OnMouseExit()
    {
        //render.material.color = initColor;
    }
    private void OnMouseDown()  //�I���Ы�Slot
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (ShopManager2.Instance.SelectedSlot == null) 
        {
            Debug.Log("SelectedSlot is null");
            return;
        }   //�P�_�p�G�Onull,�᭱�N����ާ@
        Debug.Log("�Ыت��~");


        /////�ݥ��P�_���������R�F��,�~����BuildSlot�禡
        //if (PlayS.StartMoney >= ShopManager2.Instance.SelectedSlot.cost)
        //{
        //    BuildSlot();
        //    Debug.Log("�Ѿl���B:" + PlayS.StartMoney);
        //}
        //else
        //{
        //    Debug.Log("�l�B����");
        //}
    }
    public void BuildSlot()  //Slot�Ы�
    {
        //�����ƶq���
       // PlayS.StartMoney -= ShopManager2.Instance.SelectedSlot.cost;  //�I�sShopManager2�̿�ܨ쪺���~�W����cost
        Instantiate(ShopManager2.Instance.SelectedSlot.prefab, transform.position, Quaternion.identity); //��Ҥƪ��~
    }
}
