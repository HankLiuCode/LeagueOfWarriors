using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnAttackPoint;
    public event Action OnAttackBackswing;

    public void AttackPoint()
    {
        Debug.Log("AttackPoint Animation Event On Animator");
        OnAttackPoint?.Invoke();
    }

    public void AttackBackswing()
    {
        Debug.Log("AttackBackswing Animation Event On Animator");
        OnAttackBackswing?.Invoke();
    }
}
