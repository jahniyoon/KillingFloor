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

    // 상점 오픈
    public void ShopOpen()
    {
        if (isShopOpen)
        {
            // 상점이 열려있을 땐 입력 막아주기
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.instance.inputEnable = false;
            input.cursorInputForLook = false;
            input.cursorLocked = false;

            // 상점 닫기 ESC
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
    // 상점 닫기
    public void ShopClose()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        shopUI.SetActive(false);
        PlayerUIManager.instance.shopUI.SetActive(true);
        GameManager.instance.inputEnable = true;
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
        // 플레이어가 근처에 있으면
        if (player.CompareTag("Player"))
        {
            input = player.GetComponent<PlayerInputs>();
            playerInfo = player.GetComponent<PlayerHealth>();
            shooter = player.GetComponent<PlayerShooter>();
            PlayerUIManager.instance.shopUI.SetActive(true); // 안내 UI 켜기

            if(input.equip && !isShopOpen) // 버튼을 누르면
            {
                shopUI.SetActive(true);
                PlayerUIManager.instance.shopUI.SetActive(false);
                PlayerUIManager.instance.isShopState = true;

                isShopOpen = true;
                input.equip = false;
            }

        }

    }
    // 플레이어가 근처에서 멀어지면
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
            if (playerInfo.coin >= 100 && playerInfo.armor != 100)
            {
                if (playerInfo.armor >= 100)
                {
                    float _armor = 100 - playerInfo.health;
                    playerInfo.RestoreArmor(_armor);
                    playerInfo.SpendCoin(Mathf.CeilToInt(_armor));
                }

                else
                    playerInfo.RestoreArmor(100);
                playerInfo.SpendCoin(100);
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
