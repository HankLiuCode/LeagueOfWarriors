using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoints : MonoBehaviour
{
    public static Transform[] pathPoints;  //�ŧi�@��pathPoints�Ʋ�,����pathPoints����l��
    private void Awake()  //���F���|��l�ƨӪ���,�ݼg�bAwake�̭�
    {
        pathPoints = new Transform[transform.childCount];  //�Ʋժ��ӼƬO��e���h�֭Ӥl���I,���״N�h��
        for (int i = 0; i < pathPoints.Length; i++)  //for�`����ȨC�Ӥ���
        {
            pathPoints[i] = transform.GetChild(i);  //���Ҧ����|�I(�l���I)
        }
    }
  
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
