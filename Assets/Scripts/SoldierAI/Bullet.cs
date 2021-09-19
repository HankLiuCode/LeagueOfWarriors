using UnityEngine;
using System.Collections;


public class Bullet : MonoBehaviour
{
    private GameObject target;
    public float speed = 20f;
    public Tower tower;
    void Start()
    {
        tower = GetComponentInParent<Tower>();
        //�ͦ�����1���N�n�P���A�]���i��O�l�u�w�g�s�b�����ؼСA���O�ؼж]�X�F�g�{���~�A�ҥH�ڭ̳W�w�l�u�s�b�ɶ���1��
        Destroy(this.gameObject, 1f);
    }
    /*
    //�l�u����h�L�Ϊ̭^��
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "solider")
        {

            //����H�����W����q�ȡA�եδ���k
            Health hp = other.GetComponent<Health>();
            if (hp)
            {
                hp.TakeDamage(0.5f);
                if (hp.hp.Value <= 0)
                {  //�l�u�I��p�L�A�B�p�L��q���ɬ�0�A�q�C�������p�L
                    tower.listSolider.Remove(other.gameObject);
                    Destroy(other.gameObject);
                }
                Destroy(this.gameObject);
            }

        }
        else if (other.gameObject.tag == "Player")
        {
            //����H�����W����q�ȡA�եδ���k
            Health hp = other.GetComponent<Health>();
            if (hp)
            {
                hp.TakeDamage(0.5f);
                if (hp.hp.Value <= 0)
                {  //�l�u�I��p�L�A�B�p�L��q���ɬ�0�A�q�C������
                    tower.listHero.Remove(other.gameObject);
                    Destroy(other.gameObject);
                }
                Destroy(this.gameObject);
            }

        }


    }
    */
    void Update()
    {
        //�����ؼФ����šA�l�u����
        if (target)
        {
            Vector3 dir = target.transform.position - transform.position;
            GetComponent<Rigidbody>().velocity = dir.normalized * speed;
        }
        else
        {
            //�l�u�b�V�p�L���ʪ��L�{���A�p�L�i��w�g�����F�A���ɭn�����l�u
            Destroy(this.gameObject);
        }

    }
    //�o��l�u�������ؼ�
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }


}
