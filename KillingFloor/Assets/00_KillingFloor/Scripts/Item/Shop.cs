using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    PlayerInputs input;
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
    // 상점 닫기
    public void ShopClose()
    {
        PlayerUIManager.instance.shopOpenUI.SetActive(false);
        PlayerUIManager.instance.shopUI.SetActive(true);
        isShopOpen = false;
        GameManager.instance.inputEnable = true;
        input.cursorInputForLook = true;
        input.cursorLocked = true;
        
    }
    void OnTriggerStay(Collider player)
    {
        // 플레이어가 근처에 있으면
        if (player.CompareTag("Player"))
        {
            input = player.GetComponent<PlayerInputs>();
            PlayerUIManager.instance.shopUI.SetActive(true); // 안내 UI 켜기

            if(input.equip) // 버튼을 누르면
            {
                PlayerUIManager.instance.shopOpenUI.SetActive(true);
                PlayerUIManager.instance.shopUI.SetActive(false);
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
    }
}
