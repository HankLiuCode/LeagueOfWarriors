using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; ///�I�sai����
public class Navi1 : MonoBehaviour
{
    public Transform Target1; ///�ŧi�ؼСA�᭱��Target1�i�۳ЦW��
    NavMeshAgent Agent1; ///�ŧi�ɯ�N�z��
    // Start is called before the first frame update
    void Start()
    {
        Agent1 = GetComponent<NavMeshAgent>(); ///�Ұʾɯ�
    }

    // Update is called once per frame
    void Update()
    { 
       Agent1.SetDestination(Target1.position); ///�]�w�ɯ�ؼ�
    }
}
