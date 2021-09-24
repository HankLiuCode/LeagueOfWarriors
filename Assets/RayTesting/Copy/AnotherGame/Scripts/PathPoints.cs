using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoints : MonoBehaviour
{
    public static Transform[] pathPoints;  //宣告一個pathPoints數組,先對pathPoints做初始化
    private void Awake()  //為了路徑初始化來的及,需寫在Awake裡面
    {
        pathPoints = new Transform[transform.childCount];  //數組的個數是當前有多少個子結點,長度就多少
        for (int i = 0; i < pathPoints.Length; i++)  //for循環賦值每個元素
        {
            pathPoints[i] = transform.GetChild(i);  //找到所有路徑點(子結點)
        }
    }
  
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
