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
        Debug.Log("¡ ∂RBow");
        ShopManager.Instance.SelectedTurret = bow;
    }
  public void OnPursePlate()
    {
        Debug.Log("¡ ∂RPlate");
        ShopManager.Instance.SelectedTurret = Plate;
    }
    public void OnPurseWeapon()
    {
        Debug.Log("¡ ∂RWeapon");
        ShopManager.Instance.SelectedTurret = Weapon;
    }
}
