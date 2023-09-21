using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class Shop : MonoBehaviour
{
    PlayerInputs input;
    PlayerHealth playerInfo;
    PlayerShooter shooter;
    public GameObject shopUI;
    bool isShopOpen;


    // Update is called once per frame
    void Update()
    {
        ShopOpen();
    }

    // ���� ����
    public void ShopOpen()
    {
        if (isShopOpen)
        {
            // ������ �������� �� �Է� �����ֱ�
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.instance.inputLock = true;
            input.cursorInputForLook = false;
            input.cursorLocked = false;

            // ���� �ݱ� ESC
            if(input.cancle)
            {
                ShopClose();
                input.cancle = false;
            }
        }
    }
    public void ShopCloseButton()
    {
        input.cancle = true;
    }
    // ���� �ݱ�
    public void ShopClose()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        shopUI.SetActive(false);
        PlayerUIManager.instance.shopUI.SetActive(true);
        GameManager.instance.inputLock = false;
        input.cursorInputForLook = true;
        input.cursorLocked = true;
        isShopOpen = false;
        Invoke("ChangeState", 0.5f);


    }
    private void ChangeState()
    {
        PlayerUIManager.instance.isShopState = false;
    }
    void OnTriggerStay(Collider player)
    {
        // �÷��̾ ��ó�� ������
        if (player.CompareTag("Player"))
        {
            input = player.GetComponent<PlayerInputs>();
            playerInfo = player.GetComponent<PlayerHealth>();
            shooter = player.GetComponent<PlayerShooter>();
            PlayerUIManager.instance.shopUI.SetActive(true); // �ȳ� UI �ѱ�

            if(input.equip && !isShopOpen) // ��ư�� ������
            {
                shopUI.SetActive(true);
                PlayerUIManager.instance.shopUI.SetActive(false);
                PlayerUIManager.instance.isShopState = true;

                isShopOpen = true;
                input.equip = false;
            }

        }

    }
    // �÷��̾ ��ó���� �־�����
    private void OnTriggerExit()
    {
        PlayerUIManager.instance.shopUI.SetActive(false);
        isShopOpen = false;
        input = null;
        playerInfo = null;
        shooter = null;
    }


    public void BuyArmor()
    {
        if(playerInfo != null)
        {
            // �ƸӴ� 100�� �ѵ��� ä�� �� ����
            int _armor = Mathf.FloorToInt(100 - playerInfo.armor);


            // �Ƹ� ������ 1�� 5��, 100���� ä����� 500�� �ʿ�
            // ���� �ְ� �ƸӰ� 100�� �ƴ� ��� ���� ����
            if (playerInfo.coin >= _armor * 5 && playerInfo.armor != 100)
            {

                playerInfo.RestoreArmor(_armor);
                playerInfo.SpendCoin(_armor);
            }
        }
    }

    public void BuyAmmo()
    {
        if (playerInfo != null)
        {
            if (playerInfo.coin >= 50)
            {
                shooter.GetAmmo(50);
                playerInfo.SpendCoin(50);
            }
        }
    }
}
