using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 50;  //按鍵移動畫面速度
    public float scrollSpeed = 60;  //滾輪縮放畫面遠近的速度
    public float space = 50;  //為指標移動畫面時的參考數值,預設邊的寬度50

    //限定最外圍邊界數值
    public float Min_X = -100;
    public float Max_X = 35;
    public float Min_Y = 30;
    public float Max_Y = 120;
    public float Min_Z = -50;
    public float Max_Z = 50;
    void Start()
    {
        
    }

    void Update()  //左邊為鍵盤控制畫面移動,||為指標控制畫面移動
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
        float scroll = Input.GetAxis("Mouse ScrollWheel");  //滑鼠滾輪移動(用GetAxis獲取軸)
        if (scroll != 0)
        {
            transform.position += Vector3.up * scroll * scrollSpeed * Time.deltaTime; //向上方向
        }
        Vector3 pos = transform.position;  //先獲取當前的值(操作移動完的值)

        //對x,y,z分別添加限制
        pos.x = Mathf.Clamp(pos.x, Min_X, Max_X);
        pos.y = Mathf.Clamp(pos.y, Min_Y, Max_Y);
        pos.z = Mathf.Clamp(pos.z, Min_Z, Max_Z);

        //transform.position賦值成限定後的position
        transform.position = pos;  

    }
}
