using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSound : StateMachineBehaviour
{
    public string audio;

    [Server]
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.GetComponent<Sound>().PlaySFX(audio);
        

        /*
        //��쪫��W������
        au = GameObject.Find("Arthur(Clone)").gameObject.GetComponent<AudioSource>();
        //�ϥ�Resources���i��ʺA���J �����MResources��Ƨ�����
        ac = (AudioClip)Resources.Load("Att1");
        au.clip = ac; //�N���T�ſ赹����
        au.Play();
        //au.PlayDelayed(1); //����1s�Ἵ��
        */
    }

    /*
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    
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