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
        //if (ShopManager2.Instance.SelectedSlot == null) return;  //判斷如果是null,後面就不能操作
        //render.material.color = hoverColor;  // 指標移上改變顏色
    }
    private void OnMouseExit()
    {
        //render.material.color = initColor;
    }
    private void OnMouseDown()  //點擊創建Slot
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (ShopManager2.Instance.SelectedSlot == null) 
        {
            Debug.Log("SelectedSlot is null");
            return;
        }   //判斷如果是null,後面就不能操作
        Debug.Log("創建物品");


        /////需先判斷錢夠不夠買東西,才執行BuildSlot函式
        //if (PlayS.StartMoney >= ShopManager2.Instance.SelectedSlot.cost)
        //{
        //    BuildSlot();
        //    Debug.Log("剩餘金額:" + PlayS.StartMoney);
        //}
        //else
        //{
        //    Debug.Log("餘額不足");
        //}
    }
    public void BuildSlot()  //Slot創建
    {
        //金錢數量減少
       // PlayS.StartMoney -= ShopManager2.Instance.SelectedSlot.cost;  //呼叫ShopManager2裡選擇到的物品上面的cost
        Instantiate(ShopManager2.Instance.SelectedSlot.prefab, transform.position, Quaternion.identity); //實例化物品
    }
}
