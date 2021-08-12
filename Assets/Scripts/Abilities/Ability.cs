using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "My Ability", menuName = "Abilities/Ability", order = 0)]
public class Ability : ScriptableObject
{
    [SerializeField] TargetingStrategy targetingStrategy;
    [SerializeField] FilteringStrategy[] filteringStrategies;
    [SerializeField] EffectStrategy[] effectStrategies;

    public void Use(GameObject user)
    {
        AbilityData data = new AbilityData(user);
        targetingStrategy.StartTargeting(data, () => TargetAcquired(data));
    }

    private void TargetAcquired(AbilityData data)
    {
        Debug.Log("Finished Targeting");

        if (data.GetSuccess())
        {
            foreach(FilteringStrategy filteringStrategy in filteringStrategies)
            {
                data.SetTargets(filteringStrategy.Filter(data.GetTargets()));
            }
            foreach (EffectStrategy effectStrategy in effectStrategies)
            {
                effectStrategy.StartEffect(data, EffectFinished);
            }
        }
    }

    private void EffectFinished()
    {
        Debug.Log("Effect Finished");
    }
}
