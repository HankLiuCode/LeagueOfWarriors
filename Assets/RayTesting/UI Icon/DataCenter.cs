using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCenter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Hit(float fPower , Hero hero)
    {
       float fNewHp =  hero.mproperties.fHp - fPower;
        if (fNewHp < 0)
        {
            fNewHp = 0;
        }
       
    }
}
