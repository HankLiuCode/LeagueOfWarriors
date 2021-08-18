using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DotaAnimator : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] NetworkAnimator networkAnimator;
    
    public void ResetTrigger(string trigger)
    {
        networkAnimator.ResetTrigger(trigger);
    }

    public void SetTrigger(string trigger)
    {
        networkAnimator.SetTrigger(trigger);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
}
