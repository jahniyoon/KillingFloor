using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 14);    // ������ ���� ������ ���̾� ����ũ

    public PlayerHealth playerHealth;
    public float damage = 30;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider hitObj)
    {


        // Ư�� ���̾ Ȯ��
        if (((1 << hitObj.gameObject.layer) & layerMask) != 0)
        {
            Damage(hitObj.gameObject);
        }



    }
    private void OnTriggerExit(Collider hitObj)
    {
    }

    // TODO : PunRPC�� ������ ������ ����
    void Damage(GameObject _hitObj)
    {
        if (_hitObj.transform.GetComponent<HitPoint>() == null)
        {
            playerHealth.GetCoin(100);  // Debug ����׿� ��ȭ ȹ��
            _hitObj.GetComponent<PlayerDamage>().OnDamage();
            return;
        }
        // ������ ���
        else if (_hitObj.transform.GetComponent<HitPoint>() != null)
        {
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // ���񿡰� ������

                // ���� ���� �״´ٸ�
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // ���� ���̰�
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // ���ΰ� �ʱ�ȭ
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
            }
        }
        // ������ ���
        if (_hitObj.transform.GetComponent<BossController>() != null)
        {
            // ���� ������ �־���ϴ� �κ�
            //_hitObj.transform.GetComponent<BossController>().bossHp -= damage;
        }
    }
}
