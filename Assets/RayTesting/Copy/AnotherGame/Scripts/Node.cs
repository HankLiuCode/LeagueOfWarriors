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
        if (ShopManager.Instance.SelectedTurret == null) return;  //�P�_�p�G�Onull,�᭱�N����ާ@
        render.material.color = hoverColor;  // ���в��W�����C��
    }
    private void OnMouseExit()
    {
        render.material.color = initColor;
    }
    private void OnMouseDown()  //�I���Ыد���
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (ShopManager.Instance.SelectedTurret == null) return;  //�P�_�p�G�Onull,�᭱�N����ާ@
        Debug.Log("�Ыد���");


        ///�ݥ��P�_���������R�F��,�~����BuildTurret�禡
        if (PlayerStatus.Money >= ShopManager.Instance.SelectedTurret.cost)
        {
            BuildTurret();
            Debug.Log("�Ѿl���B:" + PlayerStatus.Money);
        }
        else
        {
            Debug.Log("�l�B����");
        }
    }
    public void BuildTurret()  //����Ы�
    {
        //�����ƶq���
        PlayerStatus.Money -= ShopManager.Instance.SelectedTurret.cost;  //�I�sShopManager�̿�ܨ쪺���~�W����cost
        Instantiate(ShopManager.Instance.SelectedTurret.prefab, transform.position, Quaternion.identity); //��ҤƯ���
    }
}
