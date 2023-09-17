using Photon.Pun;
using System;
using UnityEngine;

// 생명체로서 동작할 게임 오브젝트들을 위한 뼈대를 제공
// 체력, 데미지 받아들이기, 사망 기능, 사망 이벤트를 제공
public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startingHealth = 100f; // 시작 체력
    public float health { get; protected set; } // 현재 체력
    public float armor { get; protected set; }
    public int coin { get; protected set; }
    public bool dead { get; protected set; } // 사망 상태
    public event Action OnDeath; // 사망시 발동할 이벤트


    //// 호스트->모든 클라이언트 방향으로 체력과 사망 상태를 동기화 하는 메서드
    [PunRPC]
    public void ApplyUpdatedHealth(float newHealth,float newArmor, bool newDead)
    {
        health = newHealth;
        armor = newArmor;
        dead = newDead;
    }

    // 생명체가 활성화될때 상태를 리셋
    protected virtual void OnEnable()
    {
        // 사망하지 않은 상태로 시작
        dead = false;
        // 체력을 시작 체력으로 초기화
        health = startingHealth;
        armor = 0;
    }

    // 데미지 처리
    // 호스트에서 먼저 단독 실행되고, 호스트를 통해 다른 클라이언트들에서 일괄 실행됨
    [PunRPC]
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 데미지만큼 체력 감소
            Damage(damage);

            // 호스트에서 클라이언트로 동기화
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, armor, dead);

            // 다른 클라이언트들도 OnDamage를 실행하도록 함
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        }

        // 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
        if (health <= 0 && !dead)
            if (health <= 0)
        {
            health = 0;
                Die();
            }
    }
    private void Damage(float _damage)
    {
        // 아머가 75이상이면 75% 데미지 상쇄
        if (armor >= 75)
        {
            int armorDamage = Mathf.RoundToInt(_damage * 0.75f);
            Debug.Log(armorDamage);

            armor -= Mathf.RoundToInt(_damage * 0.75f);
            health -= Mathf.RoundToInt(_damage * 0.25f);
            if (0 >= armor) armor = 0;
        }
        else if (75 >= armor && armor > 50)
        {
            armor -= Mathf.RoundToInt(_damage * 0.65f);
            health -= Mathf.RoundToInt(_damage * 0.35f);
            if (0 >= armor) armor = 0;
        }
        else if (50 >= armor && armor > 0)
        {
            armor -= Mathf.RoundToInt(_damage * 0.55f);
            health -= Mathf.RoundToInt(_damage * 0.45f);
            if (0 >= armor) armor = 0;
        }
        else
        {
            health -= _damage;
        }

    }

    // 체력을 회복하는 기능
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            // 이미 사망한 경우 체력을 회복할 수 없음
            return;
        }

        //호스트만 체력을 직접 갱신 가능
        if (PhotonNetwork.IsMasterClient)
        {
            //체력 추가
            health += newHealth;

            // 서버에서 클라이언트로 동기화
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, armor, dead);

            // 다른 클라이언트들도 RestoreHealth를 실행하도록 함
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
    }

    // 아머를 충전하는 기능
    [PunRPC]
    public virtual void RestoreArmor(float newArmor)
    {
        if(dead)
        {
            // 이미 사망한 경우 아머 회복 불가능 : 어차피 죽어서 구매도 못하지만...
            return;
        }

        //호스트만 실드를 직접 갱신 가능
        if (PhotonNetwork.IsMasterClient)
        {
            armor += newArmor;
            // 서버에서 클라이언트로 동기화
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, health, armor, dead);

            // 다른 클라이언트들도 RestoreHealth를 실행하도록 함
            photonView.RPC("RestoreArmor", RpcTarget.Others, newArmor);

        }

    }
    [PunRPC]

    public virtual void GetCoin(int newCoin)
    {
        if(dead)
        {
            return;
        }
        coin += newCoin;
    }
    public virtual void SpendCoin(int newCoin)
    {
        if(dead)
        {
            return;
        }
        coin -= newCoin;
    }


    public virtual void Die()
    {
        // onDeath 이벤트에 등록된 메서드가 있다면 실행
        if (OnDeath != null)
        {
            OnDeath();
        }

        // 사망 상태를 참으로 변경
        dead = true;
    }
}