using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float t1; ///�w�q�X��@���Ǫ�
    private float t2; ///�����w�q�ɶ�
    // Start is called before the first frame update
    void Start()
    {
        t2 = t1; ///�@�}�l�A�����w�q�ɶ�=���j�ɶ�
    }

    // Update is called once per frame
    void Update()
    {
        t2 = t2 - Time.deltaTime; ///Time.deltaTime�p�ɤu��
        Debug.Log(t2);

        if (t2 <= 0) ///�p�G�����ɶ��p��0�A�N��_�����j�ɶ�
        {
            t2 = t1;
        }
    }
}
