using Dota.Controls;
using Dota.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Health Effect", menuName = "Abilities/Effect/Health", order = 0)]
public class HealthEffect : EffectStrategy
{
    [SerializeField] float healthChange = -10f;

    [SerializeField] LayerMask effectLayer = new LayerMask();

    public override void StartEffect(AbilityData data, Action finished)
    {
        IEnumerable<GameObject> targets = data.GetTargets();
        if(targets != null)
        {
            foreach (GameObject go in targets)
            {
                Health health = go.GetComponent<Health>();
                if (health)
                {
                    health.TakeDamage(-healthChange);
                }
            }
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(data.GetTargetedPoint(), data.GetRadius(), effectLayer);
            foreach(Collider c in colliders)
            {
                Health health = c.gameObject.GetComponent<Health>();
                if (health)
                {
                    health.ClientTakeDamage(-healthChange);
                }
            }
        }
    }

    private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point, float radius, LayerMask layerMask)
    {
        RaycastHit[] hits = Physics.SphereCastAll(point, radius, Vector3.up, 0, layerMask);
        foreach (RaycastHit hit in hits)
        {
            yield return hit.collider.gameObject;
        }
    }
}
