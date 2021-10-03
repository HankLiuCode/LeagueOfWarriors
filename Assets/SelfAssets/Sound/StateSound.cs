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
        //找到物體上的元件
        au = GameObject.Find("Arthur(Clone)").gameObject.GetComponent<AudioSource>();
        //使用Resources類進行動態載入 必須和Resources資料夾對應
        ac = (AudioClip)Resources.Load("Att1");
        au.clip = ac; //將音訊剪輯給音源
        au.Play();
        //au.PlayDelayed(1); //延時1s後播放
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