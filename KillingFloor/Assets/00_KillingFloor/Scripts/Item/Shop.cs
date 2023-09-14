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

    // ���� ����
    public void ShopOpen()
    {
        if (isShopOpen)
        {
            // ������ �������� �� �Է� �����ֱ�
            GameManager.instance.inputEnable = false;
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
    // ���� �ݱ�
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
        // �÷��̾ ��ó�� ������
        if (player.CompareTag("Player"))
        {
            input = player.GetComponent<PlayerInputs>();
            PlayerUIManager.instance.shopUI.SetActive(true); // �ȳ� UI �ѱ�

            if(input.equip) // ��ư�� ������
            {
                PlayerUIManager.instance.shopOpenUI.SetActive(true);
                PlayerUIManager.instance.shopUI.SetActive(false);
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
    }
}
