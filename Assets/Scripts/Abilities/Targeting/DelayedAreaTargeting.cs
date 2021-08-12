using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;


[CreateAssetMenu(fileName = "Delayed Area Targeting", menuName = "Abilities/Targeting/Delayed Area", order = 0)]
public class DelayedAreaTargeting : TargetingStrategy
{
    [SerializeField] GameObject targetAreaPrefab = null;

    [SerializeField] LayerMask groundLayerMask = new LayerMask();

    [SerializeField] float areaAffectRadius = 2f;

    GameObject targetAreaInstance = null;

    public override void StartTargeting(AbilityData data, Action finished)
    {
        DotaPlayerController dotaPlayerController = data.GetUser().GetComponent<DotaPlayerController>();
        dotaPlayerController.StartCoroutine(Targeting(data, finished));
    }

    private IEnumerator Targeting(AbilityData data, Action finished)
    {
        ShowTargetArea();

        while (true)
        {

            if(Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit raycastHit, 1000, groundLayerMask))
            {
                targetAreaInstance.transform.position = raycastHit.point;
            }

            if (Input.GetMouseButtonDown(0))
            {
                data.SetTargetedPoint(raycastHit.point);
                data.SetRadius(areaAffectRadius);
                data.SetSuccess(true);
                break;
            }

            if (Input.GetMouseButtonDown(1))
            {
                data.SetSuccess(false);
                break;
            }

            yield return null;
        }

        targetAreaInstance.SetActive(false);
        finished();
    }

    private void ShowTargetArea()
    {
        if (targetAreaInstance == null)
            targetAreaInstance = Instantiate(targetAreaPrefab);
        else
            targetAreaInstance.SetActive(true);

        targetAreaInstance.transform.localScale = new Vector3(areaAffectRadius, 1, areaAffectRadius);
    }
}
