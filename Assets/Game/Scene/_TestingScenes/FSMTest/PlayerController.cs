using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] IdleState idleState;
    [SerializeField] WalkState walkState;
    [SerializeField] PursueState pursueState;


    private void Start()
    {
        idleState.OnStateChange += IdleState_OnStateChange;
    }

    private void IdleState_OnStateChange(State state)
    {
        idleState.Exit();
        state.Enter();
    }
}
