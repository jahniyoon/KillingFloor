using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerHealth : LivingEntity
{
    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트
    private Animator playerAnimator; // 플레이어의 애니메이터
    private PlayerInfoUI playerInfo;
    private CameraSetup playerCamera;

    private void Awake()
    {
        // 사용할 컴포넌트를 가져오기
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        playerInfo = GetComponent<PlayerInfoUI>();
        playerCamera = GetComponent<CameraSetup>();
    }

    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        // 플레이어 조작을 받는 컴포넌트들 활성화
        playerMovement.enabled = true;
        playerShooter.enabled = true;
        playerAnimator.SetBool("isDead", false);
        playerInfo.SetHealth(health);
        playerInfo.SetArmor(armor);
    }

    // 데미지 처리
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitDirection)
    {
        if (dead)
        {
            // 사망하지 않은 경우에만 효과음을 재생
            //playerAudioPlayer.PlayOneShot(hitClip);
            return;
        }

        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection);

        // 갱신된 체력 업데이트

        playerInfo.SetHealth(health);
        playerInfo.SetArmor(armor);
        playerInfo.SetBloodScreen(health);
    }
    [PunRPC]
    public override void OnPoison()
    {
        base.OnPoison();
        playerInfo.SetPoisonScreen();
    }

    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        playerInfo.SetHealth(health);
    }

    [PunRPC]
    public override void RestoreArmor(float newArmor)
    {
        base.RestoreArmor(newArmor);
        playerInfo.SetArmor(armor);
    }

    // 코인 획득
    [PunRPC]
    public override void GetCoin(int newCoin)
    {
        base.GetCoin(newCoin);
        playerInfo.SetCoin(coin);
    }
    // 코인 소비
    [PunRPC]
    public override void SpendCoin(int newCoin)
    {
        base.SpendCoin(newCoin);
        playerInfo.SetCoin(coin);
    }

    // 플레이어 죽었을 때 실행되는 것들
    public override void Die()
    {
     
        base.Die();
        playerInfo.state = PlayerInfoUI.State.Die;
        playerCamera.SetCamera();
        playerAnimator.SetBool("isDead", true);
        Invoke("Respawn",3f);
    }
    public void Respawn()
    {
        gameObject.SetActive(false);

        // 죽으면 리스폰 시키기
        if (photonView.IsMine)
        {
            Vector3 spawnPosition = new Vector3(0f, 1f, 0f);

            if (SceneManager.GetActiveScene().name == "Main")
            {
                spawnPosition = new(135.0f, -6.0f, 200.0f);
            }
            // 지정된 위치로 이동
            transform.position = spawnPosition;
        }

        // 컴포넌트들을 리셋하기 위해 게임 오브젝트를 잠시 껐다가 다시 켜기
        // 컴포넌트들의 OnDisable(), OnEnable() 메서드가 실행됨
        gameObject.SetActive(true);
        playerCamera.SetCamera();
        playerInfo.ResetScreen();
        playerInfo.state = PlayerInfoUI.State.Live;

    }

    public void BuyArmor(float _armor)
    {
        photonView.RPC("BuyArmorProcessOnServer", RpcTarget.MasterClient, _armor);
        // 마스터 클라이언트에게 구매값 요청
    }

    [PunRPC]
    public void BuyArmorProcessOnServer(float _armor)
    {
        float newArmor = armor + _armor;
        int newCoin = coin - Mathf.FloorToInt(_armor * 5);
        photonView.RPC("SyncBuyArmor", RpcTarget.All, newArmor, newCoin);
    }

    [PunRPC]
    public void SyncBuyArmor(float _armor, int _coin)
    {
        coin = _coin;
        armor = _armor;
        playerInfo.SetArmor(armor);
        playerInfo.SetCoin(coin);
    }
}
