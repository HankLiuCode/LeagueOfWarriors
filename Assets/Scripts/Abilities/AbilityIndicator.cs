using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum IndicatorType
{
    None,
    Area,
    Direction,
    Target
}

public class AbilityIndicator : MonoBehaviour
{
    [SerializeField] GameObject areaIndicatorPrefab = null;
    [SerializeField] GameObject rangeIndicatorPrefab = null;
    [SerializeField] GameObject directionIndicatorPrefab = null;

    AreaIndicator areaIndicator = null;
    AreaIndicator rangeIndicator = null;
    DirectionIndicator directionIndicator = null;


    private void Awake()
    {
        areaIndicator = Instantiate(areaIndicatorPrefab).GetComponent<AreaIndicator>();
        rangeIndicator = Instantiate(rangeIndicatorPrefab).GetComponent<AreaIndicator>();
        directionIndicator = Instantiate(directionIndicatorPrefab).GetComponent<DirectionIndicator>();

        areaIndicator.gameObject.SetActive(false);
        rangeIndicator.gameObject.SetActive(false);
        directionIndicator.gameObject.SetActive(false);
    }
}
