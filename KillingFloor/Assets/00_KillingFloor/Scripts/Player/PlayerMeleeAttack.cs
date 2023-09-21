using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 14);    // 데미지 받을 좀비의 레이어 마스크

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


        // 특정 레이어만 확인
        if (((1 << hitObj.gameObject.layer) & layerMask) != 0)
        {
            Damage(hitObj.gameObject);
        }



    }
    private void OnTriggerExit(Collider hitObj)
    {
    }

    // TODO : PunRPC로 데미지 들어가도록 수정
    void Damage(GameObject _hitObj)
    {
        if (_hitObj.transform.GetComponent<HitPoint>() == null)
        {
            playerHealth.GetCoin(100);  // Debug 디버그용 재화 획득
            _hitObj.GetComponent<PlayerDamage>().OnDamage();
            return;
        }
        // 좀비일 경우
        else if (_hitObj.transform.GetComponent<HitPoint>() != null)
        {
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // 좀비에게 데미지

                // 만약 좀비가 죽는다면
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // 코인 먹이고
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // 코인값 초기화
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
            }
        }
        // 보스일 경우
        if (_hitObj.transform.GetComponent<BossController>() != null)
        {
            // 보스 데미지 넣어야하는 부분
            //_hitObj.transform.GetComponent<BossController>().bossHp -= damage;
        }
    }
}
