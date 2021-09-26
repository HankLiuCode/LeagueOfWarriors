using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretDesign Weapon;
    public TurretDesign Plate;
    public TurretDesign bow;
    
    public void OnPurseBow()
    {
        Debug.Log("�ʶRBow");
        ShopManager.Instance.SelectedTurret = bow;
    }
  public void OnPursePlate()
    {
        Debug.Log("�ʶRPlate");
        ShopManager.Instance.SelectedTurret = Plate;
    }
    public void OnPurseWeapon()
    {
        Debug.Log("�ʶRWeapon");
        ShopManager.Instance.SelectedTurret = Weapon;
    }
}
