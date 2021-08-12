using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FilteringStrategy : ScriptableObject
{
    public abstract IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter);
}
