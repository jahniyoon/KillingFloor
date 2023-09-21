using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerHealth : LivingEntity
{
    private PlayerMovement playerMovement; // �÷��̾� ������ ������Ʈ
    private PlayerShooter playerShooter; // �÷��̾� ���� ������Ʈ
    private Animator playerAnimator; // �÷��̾��� �ִϸ�����
    private PlayerInfoUI playerInfo;

    private void Awake()
    {
        // ����� ������Ʈ�� ��������
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        playerInfo = GetComponent<PlayerInfoUI>();  
    }

    protected override void OnEnable()
    {
        // LivingEntity�� OnEnable() ���� (���� �ʱ�ȭ)
        base.OnEnable();

        // �÷��̾� ������ �޴� ������Ʈ�� Ȱ��ȭ
        playerMovement.enabled = true;
        playerShooter.enabled = true;
        PlayerUIManager.instance.SetHP(startingHealth);
        PlayerUIManager.instance.SetArmor(0);
        playerInfo.SetHealth(health);
        playerInfo.SetArmor(armor);

    }

    // ������ ó��
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitDirection)
    {
        if (!dead)
        {
            // ������� ���� ��쿡�� ȿ������ ���
            //playerAudioPlayer.PlayOneShot(hitClip);
        }

        // LivingEntity�� OnDamage() ����(������ ����)
        base.OnDamage(damage, hitPoint, hitDirection);

        // ���ŵ� ü�� ������Ʈ

        playerInfo.SetHealth(health);
        playerInfo.SetArmor(armor);
        

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
    public override void GetCoin(int newCoin)
    {
        base.GetCoin(newCoin);

    }
    // ���� �Һ�
    public override void SpendCoin(int newCoin)
    {
        base.SpendCoin(newCoin);
    }

    public override void Die()
    {
        base.Die();

        Invoke("Respawn",3f);
    }
    public void Respawn()
    {
        // ������ ������ ��Ű��
        if (photonView.IsMine)
        {
            Vector3 spawnPosition = new Vector3(0f, 1f, 0f);

            if (SceneManager.GetActiveScene().name == "Main")
            {
                spawnPosition = new(135.0f, -6.0f, 200.0f);
            }


            // ������ ���� ��ġ�� �̵�
            transform.position = spawnPosition;
        }

        // ������Ʈ���� �����ϱ� ���� ���� ������Ʈ�� ��� ���ٰ� �ٽ� �ѱ�
        // ������Ʈ���� OnDisable(), OnEnable() �޼��尡 �����
        gameObject.SetActive(false);
        gameObject.SetActive(true);

    }
}
