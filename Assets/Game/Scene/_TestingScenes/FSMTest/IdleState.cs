using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    [SerializeField] Animator animator;
    [SerializeField] LayerMask interactLayer;

    public event System.Action<State> OnStateChange;

    public override void Enter()
    {
        animator.SetBool("running", false);
    }

    public override void Execute()
    {
        Debug.Log("Idle Execute");

        if (Input.GetMouseButtonDown(0))
        {
            WalkState walkState = GetComponent<WalkState>();
            OnStateChange?.Invoke(walkState);
        }

        if (Input.GetMouseButtonDown(1))
        {
            PursueState pursueState = GetComponent<PursueState>();
            OnStateChange?.Invoke(pursueState);
        }
    }

    public override void Exit()
    {
        
    }
}
