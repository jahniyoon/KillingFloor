using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviourPun
{
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 14);    // 데미지 받을 좀비의 레이어 마스크

    public PlayerHealth playerHealth;
    public PlayerShooter shooter;
    public float damage = 80;

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
            if (photonView.IsMine)
            {
                Damage(hitObj.gameObject);
            }
            var hitTransform = hitObj.transform;
            if (hitTransform.gameObject.GetPhotonView() == null)
            {
                for (int i = 0; i < 20; i++)
                {
                    //Debug.Log("포톤뷰가 없다. 상위로 전환 : " + hitTransform);
                    hitTransform = hitTransform.parent;
                    if (hitTransform.gameObject.GetPhotonView() != null)
                    {
                        break;
                    }
                }
            }

            photonView.RPC("MeleeBlood", RpcTarget.All, this.transform.position, hitTransform.gameObject.GetPhotonView().ViewID);

        }



    }

    private void OnTriggerExit(Collider hitObj)
    {
    }

    // TODO : PunRPC로 데미지 들어가도록 수정
    void Damage(GameObject _hitObj)
    {

        Debug.Log(_hitObj.name);

        if (_hitObj.transform.GetComponent<HitPoint>() == null && _hitObj.transform.GetComponent<PlayerDamage>() != null)
        {
            playerHealth.GetCoin(100);  // Debug 디버그용 재화 획득
            _hitObj.transform.GetComponent<PlayerDamage>().OnDamage(); // RPC 확인 디버그용
            return;
        }


        if (!"Mesh_Alfa_2".Equals(_hitObj.transform.name) && !"DevilEye".Equals(_hitObj.transform.name))//보스 가 아닐경우 
        {
            ////////////////////////////////////////////////좀비////////////////////


            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // 좀비에게 데미지

                // 만약 좀비가 죽는다면
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // 코인 먹이고
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);
                    playerHealth.ExpUp(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);


                    // 코인값 초기화
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
            }

            ////////////////////////////////////////////////////////////////////
        }

        if ("Mesh_Alfa_2".Equals(_hitObj.name)) // 보스 일경우
        {


            if (9 == _hitObj.transform.gameObject.layer)
            {

                _hitObj.gameObject.GetComponent<BossController>().OnDamage(damage);
            }
            else if (11 == _hitObj.transform.gameObject.layer)
            {

                _hitObj.gameObject.GetComponent<BossController>().OnDamage(damage * 0.5f);
            }
        }

        if ("DevilEye".Equals(_hitObj.name))
        {

            Debug.Log("메테오");

            _hitObj.gameObject.GetComponent<Meteor>().OnDamage(damage);

        }
    }
}
