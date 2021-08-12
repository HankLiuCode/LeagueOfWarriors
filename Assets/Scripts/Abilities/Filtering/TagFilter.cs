using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Filters/Tag", order = 0)]
public class TagFilter : FilteringStrategy
{
    [SerializeField] string tagToFilter = "";
    public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
    {
        foreach (GameObject go in objectsToFilter)
        {
            if (go.CompareTag(tagToFilter))
            {
                yield return go;
            }
        }
    }
}
