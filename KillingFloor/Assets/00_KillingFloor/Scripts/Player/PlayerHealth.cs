using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerHealth : LivingEntity
{
    private PlayerMovement playerMovement; // �÷��̾� ������ ������Ʈ
    private PlayerShooter playerShooter; // �÷��̾� ���� ������Ʈ
    private Animator playerAnimator; // �÷��̾��� �ִϸ�����
    private PlayerInfoUI playerInfo;
    private CameraSetup playerCamera;

    private void Awake()
    {
        // ����� ������Ʈ�� ��������
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        playerInfo = GetComponent<PlayerInfoUI>();
        playerCamera = GetComponent<CameraSetup>();
    }

    protected override void OnEnable()
    {
        // LivingEntity�� OnEnable() ���� (���� �ʱ�ȭ)
        base.OnEnable();

        // �÷��̾� ������ �޴� ������Ʈ�� Ȱ��ȭ
        playerMovement.enabled = true;
        playerShooter.enabled = true;
        playerAnimator.SetBool("isDead", false);
        playerInfo.SetHealth(health);
        playerInfo.SetArmor(armor);
    }

    // ������ ó��
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitDirection)
    {
        if (dead)
        {
            // ������� ���� ��쿡�� ȿ������ ���
            //playerAudioPlayer.PlayOneShot(hitClip);
            return;
        }

        // LivingEntity�� OnDamage() ����(������ ����)
        base.OnDamage(damage, hitPoint, hitDirection);

        // ���ŵ� ü�� ������Ʈ

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

    // ���� ȹ��
    [PunRPC]
    public override void GetCoin(int newCoin)
    {
        base.GetCoin(newCoin);
        playerInfo.SetCoin(coin);
    }
    // ���� �Һ�
    [PunRPC]
    public override void SpendCoin(int newCoin)
    {
        base.SpendCoin(newCoin);
        playerInfo.SetCoin(coin);
    }

    // �÷��̾� �׾��� �� ����Ǵ� �͵�
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

        // ������ ������ ��Ű��
        if (photonView.IsMine)
        {
            Vector3 spawnPosition = new Vector3(0f, 1f, 0f);

            if (SceneManager.GetActiveScene().name == "Main")
            {
                spawnPosition = new(135.0f, -6.0f, 200.0f);
            }
            // ������ ��ġ�� �̵�
            transform.position = spawnPosition;
        }

        // ������Ʈ���� �����ϱ� ���� ���� ������Ʈ�� ��� ���ٰ� �ٽ� �ѱ�
        // ������Ʈ���� OnDisable(), OnEnable() �޼��尡 �����
        gameObject.SetActive(true);
        playerCamera.SetCamera();
        playerInfo.ResetScreen();
        playerInfo.state = PlayerInfoUI.State.Live;

    }

    public void BuyArmor(float _armor)
    {
        photonView.RPC("BuyArmorProcessOnServer", RpcTarget.MasterClient, _armor);
        // ������ Ŭ���̾�Ʈ���� ���Ű� ��û
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
