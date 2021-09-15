using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : NetworkBehaviour, ITeamMember, IMinimapEntity
{
    [SerializeField] Team team;
    [SerializeField] GameObject minimapIconPrefab = null;

    public string GetLayerName()
    {
        return "Tower";
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapTowerIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapTowerIcon>();
        minimapIconInstance.SetVisible(false);
        minimapIconInstance.SetTeam(team);

        return minimapIconInstance;
    }

    public Team GetTeam()
    {
        return team;
    }

    public void SetTeam(Team team)
    {
        this.team = team;
    }


    ////定義箭塔類型
    //public int towerType;
    ////創建一個集合，當小兵移動到箭塔範圍之內把其添加到集合裡，否則將其移除集合
    //public List<GameObject> listSolider = new List<GameObject>();
    ////創建一個集合，當英雄移動到箭塔範圍之內把其添加到集合裡，否則將其移除集合
    //public List<GameObject> listHero = new List<GameObject>();

    //[SerializeField]
    //private GameObject bulletPrefab;//子彈
    //[SerializeField]
    //private Transform bulletStart;//子彈生成時所在的位置
    //[SerializeField]
    //private Transform parent;//生成的子彈放在這個物體下

    //void Start()
    //{
    //    //有了標籤為什麼還要定義int，主要是跟小兵類型有個對應
    //    //防止我方箭塔去攻擊我方小兵
    //    if (this.gameObject.tag.Equals("Blue"))
    //    {
    //        towerType = 0;//藍方箭塔
    //    }
    //    else
    //    {
    //        towerType = 1;//紅方箭塔
    //    }
    //    InvokeRepeating("CreateBullet", 0.1f, 1.3f);
    //}
    ////生成子彈
    //public void CreateBullet()
    //{

    //    //沒有敵人，不生成子彈
    //    if (listHero.Count == 0 && listSolider.Count == 0) return;
    //    //否則生成子彈
    //    GameObject bullet = (GameObject)Instantiate(bulletPrefab, bulletStart.position, Quaternion.identity);
    //    bullet.transform.parent = parent;
    //    BulletTarget(bullet);//設置子彈攻擊目標
    //}
    ////塔是先射擊小兵，沒有小兵再射擊英雄
    //public void BulletTarget(GameObject bullet)
    //{
    //    if (listSolider.Count > 0)
    //    {
    //        //把列表裡的第一個小兵作為攻擊對象
    //        bullet.GetComponent<Bullet>().SetTarget(listSolider[0]);

    //    }
    //    else
    //    {
    //        //把列表裡的第一個英雄作為攻擊對象
    //        bullet.GetComponent<Bullet>().SetTarget(listHero[0]);

    //    }

    //}


    ////小兵進入到箭塔範圍，箭塔從集合中找攻擊目標
    //private void OnTriggerEnter(Collider other)
    //{
    //    //進入箭塔是英雄
    //    if (other.gameObject.tag == "Player")
    //    {
    //        listHero.Add(other.gameObject);
    //    }
    //    else
    //    {
    //        //進入箭塔是小兵
    //        SmartSolider solider = other.GetComponent<SmartSolider>();
    //        //當小兵不為空時，判斷小兵與箭塔類型是否一致，不一致表示可以攻擊
    //        if (solider && solider.type != towerType)
    //        {
    //            listSolider.Add(other.gameObject);//小兵放在可攻擊的列表內

    //        }

    //    }

    //}
    ////人物退出範圍，移除列表
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        listHero.Remove(other.gameObject);
    //    }
    //    else
    //    {

    //        listSolider.Remove(other.gameObject);

    //    }
    //}
}
