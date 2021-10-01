using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagPanel : MonoBehaviour
{
    private List<Transform> cells;
  
    void Start()
    {
        cells = new List<Transform>();
        int iC = this.transform.childCount;
        int i;
        Transform t;
        for (i = 0; i < iC; i++)
        {
            t = this.transform.GetChild(i);
            cells.Add(t);
        }
    }

    public int GetCellCount()
    {
        return cells.Count;
    }

    public Transform getEmptyCell()
    {
        foreach (Transform t in cells)
        {
            if (t.childCount == 0)
            {
                return t;
            }
        }
        return null;
    }

    public void PutInItem(ItemSprite sp)
    {
        Debug.Log("put");
        Transform t = getEmptyCell();
        if (t == null)
        {
            return;
        }
        Debug.Log("change parent");
        sp.transform.SetParent(t);
        sp.transform.localPosition = Vector3.zero;
    }


}