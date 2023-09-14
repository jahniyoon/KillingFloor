using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    private PlayerMovement playerMovement; // �÷��̾� ������ ������Ʈ
    private PlayerShooter playerShooter; // �÷��̾� ���� ������Ʈ
    private Animator playerAnimator; // �÷��̾��� �ִϸ�����

    private void Awake()
    {
        // ����� ������Ʈ�� ��������
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        // LivingEntity�� OnEnable() ���� (���� �ʱ�ȭ)
        base.OnEnable();

        // �÷��̾� ������ �޴� ������Ʈ�� Ȱ��ȭ
        playerMovement.enabled = true;
        playerShooter.enabled = true;

        PlayerUIManager.instance.SetHP(startingHealth);
    }

    // ������ ó��
    //[PunRPC]
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
        PlayerUIManager.instance.SetHP(health);
        PlayerUIManager.instance.SetArmor(armor);
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
        PlayerUIManager.instance.SetHP(health);
    }
    public override void RestoreArmor(float newArmor)
    {
        base.RestoreArmor(newArmor);
        PlayerUIManager.instance.SetArmor(armor);
    }

    // ���� ȹ��
    public override void GetCoin(int newCoin)
    {
        base.GetCoin(newCoin);
        CoinUpdate();
    }
    // ���� �Һ�
    public override void SpendCoin(int newCoin)
    {
        base.SpendCoin(newCoin);
        PlayerUIManager.instance.SetCoin(coin);
    }

    public void CoinUpdate()
    {
        PlayerUIManager.instance.SetCoin(coin);
    }



}
