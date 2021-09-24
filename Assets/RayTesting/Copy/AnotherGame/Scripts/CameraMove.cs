using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 50;  //���䲾�ʵe���t��
    public float scrollSpeed = 60;  //�u���Y��e�����񪺳t��
    public float space = 50;  //�����в��ʵe���ɪ��ѦҼƭ�,�w�]�䪺�e��50

    //���w�̥~����ɼƭ�
    public float Min_X = -100;
    public float Max_X = 35;
    public float Min_Y = 30;
    public float Max_Y = 120;
    public float Min_Z = -50;
    public float Max_Z = 50;
    void Start()
    {
        
    }

    void Update()  //���䬰��L����e������,||�����б���e������
    {
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x < space)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x > Screen.width - space)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y > Screen.height - space)
        {
            transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < space)
        {
            transform.position += Vector3.back * moveSpeed * Time.deltaTime;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");  //�ƹ��u������(��GetAxis����b)
        if (scroll != 0)
        {
            transform.position += Vector3.up * scroll * scrollSpeed * Time.deltaTime; //�V�W��V
        }
        Vector3 pos = transform.position;  //�������e����(�ާ@���ʧ�����)

        //��x,y,z���O�K�[����
        pos.x = Mathf.Clamp(pos.x, Min_X, Max_X);
        pos.y = Mathf.Clamp(pos.y, Min_Y, Max_Y);
        pos.z = Mathf.Clamp(pos.z, Min_Z, Max_Z);

        //transform.position��Ȧ����w�᪺position
        transform.position = pos;  

    }
}
