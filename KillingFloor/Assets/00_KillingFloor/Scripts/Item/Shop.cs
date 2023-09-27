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

    // 상점 오픈
    public void ShopOpen()
    {
        if (isShopOpen)
        {
            // 상점이 열려있을 땐 입력 막아주기
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.instance.inputLock = true;
            //input.cursorInputForLook = false;
            //input.cursorLocked = false;

            // 상점 닫기 ESC
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
    // 상점 닫기
    public void ShopClose()
    {
        Debug.Log("상점 종료 버튼이 눌리나?");
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
                    PlayerUIManager.instance.shopUI.SetActive(true); // 안내 UI 켜기

                    if (input.equip && !isShopOpen) // 버튼을 누르면
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
        // 플레이어가 근처에 있으면
      
    }
    
    // 플레이어가 근처에서 멀어지면
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
            // 아머는 100을 넘도록 채울 수 없음 구매할 아머의 양
            int _armor = Mathf.FloorToInt(100 - playerInfo.armor);
            // 돈이 없으면 구매 가능한 양만큼 구매 가능
            if(playerInfo.coin < _armor * 5)
            {
                _armor = Mathf.FloorToInt(playerInfo.coin * 5);
            }


            Debug.Log("플레이어 돈 : "+playerInfo.coin+"플레이어 아머 : "+playerInfo.armor);
            // 아머 가격은 1당 5원, 100까지 채우려면 500원 필요
            // 돈이 있고 아머가 100이 아닐 경우 구매 가능
            if (playerInfo.coin >= _armor * 5 && playerInfo.armor != 100)
            {
                Debug.Log("구입하나?");
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
