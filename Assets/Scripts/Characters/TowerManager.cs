using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TowerManager : NetworkBehaviour
{
    public event System.Action<Tower> OnTowerAdded;
    public event System.Action<Tower> OnTowerRemoved;

    void Start()
    {
        
    }

    public void AddTower(Tower tower)
    {
        OnTowerAdded?.Invoke(tower);
    }

    public void RemoveTower(Tower tower)
    {
        OnTowerRemoved?.Invoke(tower);
    }
}
