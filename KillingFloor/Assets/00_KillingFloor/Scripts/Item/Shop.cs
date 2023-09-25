using JetBrains.Annotations;
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
        if(playerInfo != null && playerInfo.photonView.IsMine)
        { 
            ShopOpen();
        }
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
            //input.cursorInputForLook = false;
            //input.cursorLocked = false;

            // ���� �ݱ� ESC
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ShopClose();
                input.cancle = false;
            }
            else if(!GameManager.instance.isShop)
            {
                ShopClose();
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
        Debug.Log("���� ���� ��ư�� ������?");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        shopUI.SetActive(false);
        PlayerUIManager.instance.shopUI.SetActive(true);
        GameManager.instance.inputLock = false;
        //input.cursorInputForLook = true;
        //input.cursorLocked = true;
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
        if (player.CompareTag("Player") && GameManager.instance.isShop)
        {
            input = player.GetComponent<PlayerInputs>();
            playerInfo = player.GetComponent<PlayerHealth>();
            shooter = player.GetComponent<PlayerShooter>();
            if (playerInfo != null && playerInfo.photonView.IsMine)
            {
                PlayerUIManager.instance.shopUI.SetActive(true); // �ȳ� UI �ѱ�

                if (input.equip && !isShopOpen) // ��ư�� ������
                {
                    shopUI.SetActive(true);
                    PlayerUIManager.instance.shopUI.SetActive(false);
                    PlayerUIManager.instance.isShopState = true;

                    isShopOpen = true;
                    input.equip = false;
                }
            }

        }
    }
    
    // �÷��̾ ��ó���� �־�����
    private void OnTriggerExit()
    {
        if (playerInfo != null &&playerInfo.photonView.IsMine && GameManager.instance.isShop)
        {
            PlayerUIManager.instance.shopUI.SetActive(false);
            isShopOpen = false;
            input = null;
            playerInfo = null;
            shooter = null;
        }
    }


    public void BuyArmor()
    {
        if(playerInfo != null)
        {
            // �ƸӴ� 100�� �ѵ��� ä�� �� ���� ������ �Ƹ��� ��
            int _armor = Mathf.FloorToInt(100 - playerInfo.armor);
            // ���� ������ ���� ������ �縸ŭ ���� ����
            if(playerInfo.coin < _armor * 5)
            {
                _armor = Mathf.FloorToInt(playerInfo.coin * 5);
            }


            Debug.Log("�÷��̾� �� : "+playerInfo.coin+"�÷��̾� �Ƹ� : "+playerInfo.armor);
            // �Ƹ� ������ 1�� 5��, 100���� ä����� 500�� �ʿ�
            // ���� �ְ� �ƸӰ� 100�� �ƴ� ��� ���� ����
            if (playerInfo.coin >= _armor * 5 && playerInfo.armor != 100)
            {
                Debug.Log("�����ϳ�?");
                //playerInfo.RestoreArmor(_armor);
                //playerInfo.SpendCoin(_armor);
                playerInfo.BuyArmor(_armor);
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
                playerInfo.BuyAmmo(50);
            }
        }
    }
}
