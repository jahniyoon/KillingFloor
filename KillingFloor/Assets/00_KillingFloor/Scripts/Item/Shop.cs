using JetBrains.Annotations;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class Shop : MonoBehaviour
{
    PlayerInputs input;
    PlayerHealth playerInfo;
    PlayerShooter shooter;

    [Header("Shop State")]
    public GameObject shopUI;
    bool isShopOpen;

    public TMP_Text playerCoin;
    public TMP_Text wave;
    public TMP_Text waveTime;

    [SerializeField] private int MaxAmmo;
    [SerializeField] private int remaining;
    [SerializeField] private int magazineAmmo;
    [SerializeField] private int waveShop;
    [SerializeField] private int subWaveShop;
    // Update is called once per frame
    void Update()
    {

        if (playerInfo.photonView.IsMine)
        {
            ShopUpdate();
            if (playerInfo != null)
            {
                ShopOpen();
            }
        }
    }

    public void ShopUpdate()
    {
        waveTime.text = PlayerUIManager.instance.timerCountText.text;
        wave.text = PlayerUIManager.instance.zombieWaveText.text;
        playerCoin.text = PlayerUIManager.instance.coinText.text;
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
        MaxAmmo =(int)player.GetComponent<PlayerShooter>().equipedWeapon.maxAmmo;
        remaining = (int)player.GetComponent<PlayerShooter>().equipedWeapon.remainingAmmo;
        magazineAmmo = (int)player.GetComponent<PlayerShooter>().equipedWeapon.magazineSize;
        if(GameManager.instance.wave == waveShop || GameManager.instance.wave == subWaveShop)
        {
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
        // �÷��̾ ��ó�� ������
      
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
         
            if (playerInfo.coin >= 200 && MaxAmmo > remaining)
            {
                if (magazineAmmo + remaining > MaxAmmo)
                {
                    shooter.GetAmmo(remaining- MaxAmmo);
                }else
                {
                    shooter.GetAmmo(magazineAmmo);

                }
               
                playerInfo.BuyAmmo(200);
            }
        }
    }
}
