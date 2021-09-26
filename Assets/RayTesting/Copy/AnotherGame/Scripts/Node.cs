using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    private Color hoverColor = Color.gray;
    private Color initColor;
    private Renderer render;
    void Start()
    {
        render = GetComponent<MeshRenderer>();  
        initColor = render.material.color;
    }

    
    void Update()
    {
        
    }
    private void OnMouseEnter()
    {
        if(EventSystem.current.IsPointerOverGameObject()) return;
        if (ShopManager.Instance.SelectedTurret == null) return;  //判斷如果是null,後面就不能操作
        render.material.color = hoverColor;  // 指標移上改變顏色
    }
    private void OnMouseExit()
    {
        render.material.color = initColor;
    }
    private void OnMouseDown()  //點擊創建砲塔
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (ShopManager.Instance.SelectedTurret == null) return;  //判斷如果是null,後面就不能操作
        Debug.Log("創建砲塔");


        ///需先判斷錢夠不夠買東西,才執行BuildTurret函式
        if (PlayerStatus.Money >= ShopManager.Instance.SelectedTurret.cost)
        {
            BuildTurret();
            Debug.Log("剩餘金額:" + PlayerStatus.Money);
        }
        else
        {
            Debug.Log("餘額不足");
        }
    }
    public void BuildTurret()  //砲塔創建
    {
        //金錢數量減少
        PlayerStatus.Money -= ShopManager.Instance.SelectedTurret.cost;  //呼叫ShopManager裡選擇到的物品上面的cost
        Instantiate(ShopManager.Instance.SelectedTurret.prefab, transform.position, Quaternion.identity); //實例化砲塔
    }
}
