using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
//using UnityEngine.Windows;
using static Cinemachine.DocumentationSortingAttribute;
using static UnityEngine.Rendering.DebugUI;

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

    
    public void Update()
    {
        SecretCode();
    }
    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        // 플레이어 조작을 받는 컴포넌트들 활성화
        level =int.Parse(NetworkManager.net_instance.localPlayerLv);
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

    [PunRPC]
    public override void ExpUp(int value)
    {
        base.ExpUp(value);
        playerInfo.SetExp(exp);
        playerInfo.SetLevel(level);
    }
    public override void LevelUp()
    {
        base.LevelUp();
        NetworkManager.net_instance.SetData(string.Format("{0}", level));
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

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerInfo.state = PlayerInfoUI.State.Die;
        playerCamera.SetCamera();
        
        
        Invoke("PlayerDisable", 3f);

        playerAnimator.SetBool("isDead", true);

        // 죽으면 마스터에게 죽었다고 알림
        photonView.RPC("DieProcessOnServer", RpcTarget.MasterClient);
        //Invoke("Respawn",3f);
    }
    public void PlayerDisable()
    {
        // 게임 오버 상태가 아니면
        if (!GameManager.instance.isGameover)
        {
            for (int i = 0; i <= GameManager.instance.targetPlayer.Length; i++)
            {
                
                // 누군가 살아있으면
                if (GameManager.instance.targetPlayer[i].GetComponent<PlayerHealth>().dead == false)
                {
                    playerInfo.ResetScreen();
                    playerCamera.InspectorCam(GameManager.instance.targetPlayer[i]);
                    break;
                }
            }
        }

        transform.position = new Vector3(0, -100, 0);
    }
    [PunRPC]
    public void DieProcessOnServer()
    {
        // 마스터는 죽은 사람의 수를 계산하고 보내줌
        int dieCount = GameManager.instance.playerDieCount + 1;
        photonView.RPC("DieSync", RpcTarget.All, dieCount);
    }
    [PunRPC]
    public void DieSync(int _dieCount)
    {
        GameManager.instance.playerDieCount = _dieCount;

        // 만약 지금 있는 플레이어보다 카운트가 높거나 같으면 게임오버 실행
        if(GameManager.instance.playerDieCount >= GameManager.instance.targetPlayer.Length)
        {
            GameManager.instance.OnGameOver();
        }
    }

    public void Respawn()
    {

        // 죽으면 리스폰 시키기
        if (photonView.IsMine)
        {
            Vector3 spawnPosition = new Vector3(0f, 1f, 0f);

            if (SceneManager.GetActiveScene().name == "Main")
            {
                spawnPosition = new(-22.639f, -5.9f, 22.1f);
            }
            // 지정된 위치로 이동
            transform.position = spawnPosition;
        }

        // 컴포넌트들을 리셋하기 위해 게임 오브젝트를 잠시 껐다가 다시 켜기
        // 컴포넌트들의 OnDisable(), OnEnable() 메서드가 실행됨
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        playerCamera.SetCamera();
        playerInfo.ResetScreen();
        playerInfo.state = PlayerInfoUI.State.Live;

    }
    public void BuyAmmo(int _coin)
    {
        photonView.RPC("BuyAmmoProcessOnServer", RpcTarget.MasterClient, _coin);

    }
    [PunRPC]
    public void BuyAmmoProcessOnServer(int _coin)
    {
        int newCoin = coin - _coin;
        photonView.RPC("SyncBuyAmmo", RpcTarget.All, newCoin);
    }

    [PunRPC]
    public void SyncBuyAmmo(int _coin)
    {
        coin = _coin;
        playerInfo.SetCoin(coin);
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
    public void SecretCode()
    {
        // ******** SecretCode ******** //
        //                              //
        //       F1 : 체력 회복          //
        //       F2 : 아머 회복          //
        //       F3 : 5000 코인 획득     //
        //       F4 : 100 탄창 획득      //
        //       F5 : SMG 구매          //
        //       F6 : SCAR 구매         //
        //                             //
        // *************************** //


        if (Input.GetKeyDown(KeyCode.F1))
        {
            float NewHealth = 100 - health;
            RestoreHealth(NewHealth);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            float newArmor = 100 - armor;
            RestoreArmor(newArmor);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GetCoin(5000);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            playerShooter.GetAmmo(100);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            playerShooter.BuySMG();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            playerShooter.BuySCAR();
        }
    }
}
