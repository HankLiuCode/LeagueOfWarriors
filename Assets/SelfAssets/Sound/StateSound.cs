using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSound : StateMachineBehaviour
{
    public string audio;

    [Client]
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Sound>().PlaySFX(audio);
    }
}