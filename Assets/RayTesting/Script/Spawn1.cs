using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn1 : MonoBehaviour
{
    public GameObject nagent; ///���ͩǪ��W�١Anagent�i�۳ЦW
    public GameObject goal; ///�s���ͩǪ��ؼСAtarget�i�۳ЦW
    // Start is called before the first frame update
    void Start()
    {
        Invoke("spawn1",3); ///��l����3��Aspawn1�i�۳ЦW
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void spawn1()
    {
        Instantiate(nagent, transform.position, transform.rotation);
        Invoke("spawn1", Random.Range(1, 5)); ///1~5�üƲ���                                                                
    }
}
