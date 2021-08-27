using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public HeroData mproperties;
    public System.Action<float>mUICallback;
    // Start is called before the first frame update
    void Start()
    {
        mproperties = new HeroData();
        mproperties.fHp = 100.0f;
        mproperties.fMp = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateHp(float fHp)
    {
        mproperties.fHp = fHp;
        mUICallback(fHp);
    }
}
