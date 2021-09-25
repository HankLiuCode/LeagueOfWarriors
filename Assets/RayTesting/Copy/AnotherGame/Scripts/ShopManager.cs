using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    private TurretDesign selectedTurret;
    public TurretDesign SelectedTurret
    {
        get
        {
            return selectedTurret;
        }
        set
        {
            selectedTurret = value;
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
