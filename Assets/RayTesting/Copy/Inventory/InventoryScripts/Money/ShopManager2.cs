using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager2 : MonoBehaviour
{
    public static ShopManager2 Instance;
    private SlotDesign selectedSlot;
    public SlotDesign SelectedSlot
    {
        get
        {
            return selectedSlot;
        }
        set
        {
            selectedSlot = value;
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
