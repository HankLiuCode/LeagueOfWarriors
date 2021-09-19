using UnityEngine;
using System.Collections;

public class CreatSoldierRed : MonoBehaviour
{
    [SerializeField]
    GameObject soliderPrefab;
    //紅方小兵生成點有3個
    [SerializeField]
    Transform[] Start2;

    [SerializeField]
    Transform soliderParent;
    //是否生成小兵默認為true
    bool isCreateSolider = true;
    //生成小兵的數量
    public int soliderCount = 2;


    //定義紅方需要去的3個路線
    [SerializeField]
    private Transform[] middleBlueTowers;
    [SerializeField]
    private Transform[] LeftBlueTowers;
    [SerializeField]
    private Transform[] RightBlueTowers;


    private void Start()
    {
        StartCoroutine(Create(0, 1, 10));
    }


    //協程生成一波一波小兵
    /// <summary>
    /// </summary>
    /// <param name="time">遊戲開始後幾秒開始生成士兵</param>
    /// <param name="delyTime">同一波內兩個小兵生成的間隔</param>
    /// <param spwanTime="">下一波小兵生成的時間間隔</param>
    /// <returns></returns>
    IEnumerator Create(float time, float delyTime, float spwanTime)
    {
        yield return new WaitForSeconds(time);
        while (isCreateSolider)
        {
            //一個for循環代表一波小兵
            for (int i = 0; i < soliderCount; i++)
            {
                //1 << 3 表示2的3次方,指的是中路
                CreateSmartSolider(SoldierType.soldier2, Start2[0], middleBlueTowers, 1 << 3);//中路紅方小兵

                CreateSmartSolider(SoldierType.soldier2, Start2[1], LeftBlueTowers, 1 << 4);//（左）路紅方小兵

                CreateSmartSolider(SoldierType.soldier2, Start2[2], RightBlueTowers, 1 << 5);//（右）路紅方小兵


                yield return new WaitForSeconds(delyTime);
            }
            //等待下一波小兵生成的時間
            yield return new WaitForSeconds(spwanTime);
        }
    }

    //生成一個小兵
    /// <summary>
    /// 生成小兵的類型
    /// 小兵生成的位置
    /// 小兵需要到達的目標點
    /// 小兵需要走哪條路
    /// </summary>
    /// <param name="startTran"></param>
    /// <param name="towers"></param>
    void CreateSmartSolider(SoldierType soldierType, Transform startTran, Transform[] towers, int road)
    {
        GameObject obj = Instantiate(soliderPrefab, startTran.position, Quaternion.identity) as GameObject;
        obj.transform.parent = soliderParent;//生成的小兵指定父物體
        //給生成的小兵指定目標點對象,
        SmartSolider solider = obj.GetComponent<SmartSolider>();
        solider.towers = towers;
        solider.SetRoad(road);
        solider.type = (int)soldierType;
    }
}
