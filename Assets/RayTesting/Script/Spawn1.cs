using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn1 : MonoBehaviour
{
    public GameObject nagent; ///產生怪物名稱，nagent可自創名
    public GameObject goal; ///新產生怪物目標，target可自創名
    // Start is called before the first frame update
    void Start()
    {
        Invoke("spawn1",3); ///初始延遲3秒，spawn1可自創名
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void spawn1()
    {
        Instantiate(nagent, transform.position, transform.rotation);
        Invoke("spawn1", Random.Range(1, 5)); ///1~5亂數產生                                                                
    }
}
