using UnityEngine;
using System.Collections;

public class CreatMonster : MonoBehaviour
{
    [SerializeField] GameObject monsterPrefab;

    //設定怪物生成點
    [SerializeField] Transform StartPosition;
    
    public float direction; //設定怪物面朝方向

    

    [SerializeField]
    Transform monsterParent;
    //是否生成怪物默認為true
    bool isCreatMonster = true;

    public void CreateMonsters() 
    {
        StartCoroutine(Create(10)); //設定怪物重生時間為10秒
    }
    public void Start()
    {
        CreateMonsters();
    }
    

    //生成怪物
    /// <summary>
    /// </summary>
    /// <param name="time">幾秒後生成怪物</param>
    /// <returns></returns>
    IEnumerator Create(float time)
    {
        yield return new WaitForSeconds(time);
        if (isCreatMonster)
        {
            CreateMonster(StartPosition);
        }
    }
    

    //生成一個怪物
    /// <summary>
    /// 怪物生成的位置
    /// </summary>
    /// <param name="startTran"></param>
    void CreateMonster(Transform startTran)
    {
        GameObject obj = Instantiate(monsterPrefab, startTran.position, Quaternion.Euler(0, direction, 0)) as GameObject;
        obj.transform.parent = monsterParent;//生成的怪物指定父物體

        //生成的怪物取得該重生點資訊
        MonsterAI monst = obj.GetComponent<MonsterAI>();
        //monst.RebirthPrefab = this.gameObject;
    }

}
