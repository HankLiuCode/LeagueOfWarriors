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
        OnAttackPoint?.Invoke();
    }

    public void AttackBackswing()
    {
        OnAttackBackswing?.Invoke();
    }
}
