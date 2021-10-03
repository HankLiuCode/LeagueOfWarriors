using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particles : StateMachineBehaviour
{
    public GameObject particle;

    protected GameObject effectParticle;




    [Server]
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        effectParticle = Instantiate(particle, animator.transform.position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(effectParticle);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(effectParticle);
        NetworkServer.Destroy(effectParticle);
    }


    /*IEnumerator DestroyEffectParticle(float seconds, GameObject effectParticle)
{
    yield return new WaitForSeconds(seconds);
    NetworkServer.Destroy(effectParticle);
}*/

    /*
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("On Attack Update ");
    }
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("On Attack Move ");
    }
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("On Attack IK ");
    }*/
}