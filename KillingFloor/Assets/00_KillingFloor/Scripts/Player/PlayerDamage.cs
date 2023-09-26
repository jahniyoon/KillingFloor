using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviourPun
{

    public float damage = 20f; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime = 0; // 마지막 공격 시점
    public int destroyCount;

    public bool isPoison;

    // ================== 데미지를 입는 과정 정리 ================== //
    // 1. 어디선가 데미지 메서드를 호출한다.
    // 마스터에게 데미지 계산을 요청한다.
    public void OnDamage()
    {
        // 마스터에게 데미지 계산 요청
        //Debug.Log("데미지 계산요청" + photonView.ViewID);

        photonView.RPC("MasterDamage", RpcTarget.MasterClient, destroyCount);
    }
    // 2.마스터가 데미지 계산을 요청받고 계산을 먼저 해준다.
    // 계산이 끝난 값을 모두에게 보내준다.
    [PunRPC]
    public void MasterDamage(int _destroyCount)
    {
        //Debug.Log("마스터 모두에게 데미지 업데이트 요청" );

        // 5번 데미지 요청 받으면 파괴도록하기위해 카운트 1 증가
        int newDestroyCount = _destroyCount + 1;
        // 마스터가 계산한 값 전달
        photonView.RPC("SyncDamage", RpcTarget.All, newDestroyCount);

    }
    // 3. 모두는 (마스터를 포함) 전달받은 값을 업데이트를 한다.
    [PunRPC]
    public void SyncDamage(int _destroyCount)
    {
        // 마스터가 계산한 값을 모두 받아와서 업데이트
        destroyCount = _destroyCount;

        if (5 < destroyCount)
        {
            //Debug.Log("게임오브젝트 제거");
            PhotonNetwork.Destroy(gameObject);
        }
    }
    // ================== 데미지를 입는 과정 정리 ================== //

    private void OnTriggerStay(Collider other)
    {
        // 상대방으로부터 LivingEntity 타입을 가져오기 시도
        LivingEntity attackTarget
            = other.GetComponent<LivingEntity>();

        // 마스터가 아니라면 데미지 입력 불가
        // 최근 공격 시점에서 timeBetAttack 이상 시간이 지났다면 공격 가능
        if (Time.time >= lastAttackTime + timeBetAttack)
        {
            // 상대방의 LivingEntity가 자신의 추적 대상이라면 공격 실행
            if (attackTarget != null)
            {
                // 실제 공격 처리는 마스터에게만 전달
                if (PhotonNetwork.IsMasterClient)
                {// 최근 공격 시간을 갱신
                    lastAttackTime = Time.time;

                    // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    Vector3 hitNormal = transform.position - other.transform.position;

                    // 공격 실행
                    attackTarget.OnDamage(damage, hitPoint, hitNormal);
                    if (isPoison)
                    {
                        attackTarget.OnPoison();
                    }
                }
               
            }
        }
    }
}
