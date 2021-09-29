using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain2 : MonoBehaviour
{
    static private UIMain2 mInstance;
    static public UIMain2 Instance() { return mInstance; }

    public BagPanel BagP;

    private void Awake()
    {
        mInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void putItemToBagPanel(ItemSprite sp)
    {
        BagP.PutInItem(sp);
    }
}