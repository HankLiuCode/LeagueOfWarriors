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


    ////�w�q�b������
    //public int towerType;
    ////�Ыؤ@�Ӷ��X�A��p�L���ʨ�b��d�򤧤����K�[�춰�X�̡A�_�h�N�䲾�����X
    //public List<GameObject> listSolider = new List<GameObject>();
    ////�Ыؤ@�Ӷ��X�A��^�����ʨ�b��d�򤧤����K�[�춰�X�̡A�_�h�N�䲾�����X
    //public List<GameObject> listHero = new List<GameObject>();

    //[SerializeField]
    //private GameObject bulletPrefab;//�l�u
    //[SerializeField]
    //private Transform bulletStart;//�l�u�ͦ��ɩҦb����m
    //[SerializeField]
    //private Transform parent;//�ͦ����l�u��b�o�Ӫ���U

    //void Start()
    //{
    //    //���F���Ҭ������٭n�w�qint�A�D�n�O��p�L�������ӹ���
    //    //����ڤ�b��h�����ڤ�p�L
    //    if (this.gameObject.tag.Equals("Blue"))
    //    {
    //        towerType = 0;//�Ť�b��
    //    }
    //    else
    //    {
    //        towerType = 1;//����b��
    //    }
    //    InvokeRepeating("CreateBullet", 0.1f, 1.3f);
    //}
    ////�ͦ��l�u
    //public void CreateBullet()
    //{

    //    //�S���ĤH�A���ͦ��l�u
    //    if (listHero.Count == 0 && listSolider.Count == 0) return;
    //    //�_�h�ͦ��l�u
    //    GameObject bullet = (GameObject)Instantiate(bulletPrefab, bulletStart.position, Quaternion.identity);
    //    bullet.transform.parent = parent;
    //    BulletTarget(bullet);//�]�m�l�u�����ؼ�
    //}
    ////��O���g���p�L�A�S���p�L�A�g���^��
    //public void BulletTarget(GameObject bullet)
    //{
    //    if (listSolider.Count > 0)
    //    {
    //        //��C��̪��Ĥ@�Ӥp�L�@��������H
    //        bullet.GetComponent<Bullet>().SetTarget(listSolider[0]);

    //    }
    //    else
    //    {
    //        //��C��̪��Ĥ@�ӭ^���@��������H
    //        bullet.GetComponent<Bullet>().SetTarget(listHero[0]);

    //    }

    //}


    ////�p�L�i�J��b��d��A�b��q���X��������ؼ�
    //private void OnTriggerEnter(Collider other)
    //{
    //    //�i�J�b��O�^��
    //    if (other.gameObject.tag == "Player")
    //    {
    //        listHero.Add(other.gameObject);
    //    }
    //    else
    //    {
    //        //�i�J�b��O�p�L
    //        SmartSolider solider = other.GetComponent<SmartSolider>();
    //        //��p�L�����ŮɡA�P�_�p�L�P�b�������O�_�@�P�A���@�P��ܥi�H����
    //        if (solider && solider.type != towerType)
    //        {
    //            listSolider.Add(other.gameObject);//�p�L��b�i�������C��

    //        }

    //    }

    //}
    ////�H���h�X�d��A�����C��
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
