using UnityEngine;
using System.Collections;

public class CreatSoldierBlue : MonoBehaviour
{
    [SerializeField]
    GameObject soliderPrefab;
    //�Ť�p�L�ͦ��I��3��
    [SerializeField]
    Transform[] Start1;

    [SerializeField]
    Transform soliderParent;
    //�O�_�ͦ��p�L�q�{��true
    bool isCreateSolider = true;
    //�ͦ��p�L���ƶq
    public int soliderCount = 2;


    //�w�q�Ť�ݭn�h��3�Ӹ��u
    [SerializeField]
    private Transform[] middleRedTowers;
    [SerializeField]
    private Transform[] LeftRedTowers;
    [SerializeField]
    private Transform[] RightRedTowers;


    private void Start()
    {
        StartCoroutine(Create(0, 1, 10));
    }


    //��{�ͦ��@�i�@�i�p�L
    /// <summary>
    /// </summary>
    /// <param name="time">�C���}�l��X��}�l�ͦ��h�L</param>
    /// <param name="delyTime">�P�@�i����Ӥp�L�ͦ������j</param>
    /// <param spwanTime="">�U�@�i�p�L�ͦ����ɶ����j</param>
    /// <returns></returns>
    IEnumerator Create(float time, float delyTime, float spwanTime)
    {
        yield return new WaitForSeconds(time);
        while (isCreateSolider)
        {
            //�@��for�`���N��@�i�p�L
            for (int i = 0; i < soliderCount; i++)
            {
                //1 << 3 ���2��3����,�����O����
                CreateSmartSolider(SoldierType.soldier1, Start1[0], middleRedTowers, 1 << 3);//�����Ť�p�L
                
                CreateSmartSolider(SoldierType.soldier1, Start1[1], LeftRedTowers, 1 << 4);//�]���^���Ť�p�L
                
                CreateSmartSolider(SoldierType.soldier1, Start1[2], RightRedTowers, 1 << 5);//�]�k�^���Ť�p�L
                


                yield return new WaitForSeconds(delyTime);
            }
            //���ݤU�@�i�p�L�ͦ����ɶ�
            yield return new WaitForSeconds(spwanTime);
        }
    }

    //�ͦ��@�Ӥp�L
    /// <summary>
    /// �ͦ��p�L������
    /// �p�L�ͦ�����m
    /// �p�L�ݭn��F���ؼ��I
    /// �p�L�ݭn��������
    /// </summary>
    /// <param name="startTran"></param>
    /// <param name="towers"></param>
    void CreateSmartSolider(SoldierType soldierType, Transform startTran, Transform[] towers, int road)
    {
        GameObject obj = Instantiate(soliderPrefab, startTran.position, Quaternion.identity) as GameObject;
        obj.transform.parent = soliderParent;//�ͦ����p�L���w������
        //���ͦ����p�L���w�ؼ��I��H,
        SmartSolider solider = obj.GetComponent<SmartSolider>();
        solider.towers = towers;
        solider.SetRoad(road);
        solider.type = (int)soldierType;
    }

}
