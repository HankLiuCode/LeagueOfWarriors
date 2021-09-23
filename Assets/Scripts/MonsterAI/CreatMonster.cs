using UnityEngine;
using System.Collections;

public class CreatMonster : MonoBehaviour
{
    [SerializeField] GameObject monsterPrefab;

    //�]�w�Ǫ��ͦ��I
    [SerializeField] Transform StartPosition;
    
    public float direction; //�]�w�Ǫ����¤�V

    

    [SerializeField]
    Transform monsterParent;
    //�O�_�ͦ��Ǫ��q�{��true
    bool isCreatMonster = true;

    public void CreateMonsters() 
    {
        StartCoroutine(Create(10)); //�]�w�Ǫ����ͮɶ���10��
    }
    public void Start()
    {
        CreateMonsters();
    }
    

    //�ͦ��Ǫ�
    /// <summary>
    /// </summary>
    /// <param name="time">�X���ͦ��Ǫ�</param>
    /// <returns></returns>
    IEnumerator Create(float time)
    {
        yield return new WaitForSeconds(time);
        if (isCreatMonster)
        {
            CreateMonster(StartPosition);
        }
    }
    

    //�ͦ��@�өǪ�
    /// <summary>
    /// �Ǫ��ͦ�����m
    /// </summary>
    /// <param name="startTran"></param>
    void CreateMonster(Transform startTran)
    {
        GameObject obj = Instantiate(monsterPrefab, startTran.position, Quaternion.Euler(0, direction, 0)) as GameObject;
        obj.transform.parent = monsterParent;//�ͦ����Ǫ����w������

        //�ͦ����Ǫ����o�ӭ����I��T
        MonsterAI monst = obj.GetComponent<MonsterAI>();
        //monst.RebirthPrefab = this.gameObject;
    }

}
