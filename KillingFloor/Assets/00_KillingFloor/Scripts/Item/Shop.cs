using JetBrains.Annotations;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class Shop : MonoBehaviour
{
    PlayerInputs input;
    PlayerHealth playerInfo;
    PlayerShooter shooter;

    [Header("Shop State")]
    public GameObject shopUI;
    bool isShopOpen;

    [Header("Shop Info")]
    public TMP_Text playerCoin;
    public TMP_Text wave;
    public TMP_Text waveTime;

    [SerializeField] private int MaxAmmo;
    [SerializeField] private int remaining;
    [SerializeField] private int magazineAmmo;
    [SerializeField] private int waveShop;
    [SerializeField] private int subWaveShop;

    [Header("Armor")]
    public Text armorValueText;     // 아머 값
    public Text armorPriceText;     // 아머 가격
    public Button buyArmorBtn;
    public GameObject buyArmorDisableBtn;

    [Header("Grenade")]
    public Text grenadeValueText;   // 수류탄 값
    public Text grenadePriceText;   // 수류탄 가격
    public Button buyGrenadeBtn;
    public GameObject buyGrenadeDisableBBtn;

    [Header("Pistol")]
    public Text pistolValueText;    // 권총 값
    public Text pistolPriceText;    // 권총 가격
    public Text pistolNameText;
    public Button buyPistolBtn;
    public GameObject buyPistolDisableBtn;
    public GameObject[] pistolIcon;

    [Header("Rifle")]
    public Text rifleValueText;     // 주무기 값
    public Text riflePriceText;     // 주무기 가격
    public Text rifleNameText;
    public Button buyRifleBtn;
    public GameObject buyRifleDisableBtn;
    public GameObject[] rifleIcon;

    [Header("SubWeapon")]
    public Text subWeaponValueText;     // 주무기 값
    public Text subWeaponPriceText;     // 주무기 가격
    public Text subWeaponNameText;
    public Button buysubWeaponBtn;
    public GameObject buysubWeaponDisableBtn;
    public GameObject[] subWeaponIcon;


    [Header("BuyWeapon")]
    public Button buyScarBtn;
    public GameObject buyScarDisableBtn;
    public Text SmgBtnText;
    public Button buySmgBtn;
    public GameObject buySmgDisableBtn;
    public Text ScarBtnText;



    // Update is called once per frame
    void Update()
    {

        // 플레이어의 정보가 있고, 포톤 IsMine일 경우 업데이트
        if (playerInfo != null &&playerInfo.photonView.IsMine)
        {
            ShopUpdate();
            
            ShopOpen();
        }
    }

    public void ShopUpdate()
    {
        waveTime.text = PlayerUIManager.instance.timerCountText.text;   // 웨이브 시간 업데이트
        wave.text = PlayerUIManager.instance.zombieWaveText.text;       // 웨이브 몇인지 업데이트
        playerCoin.text = PlayerUIManager.instance.coinText.text;       // 플레이어 금액 업데이트


        //  ================================================ 아머 업데이트 ================================================
        armorValueText.text = string.Format("{0}/100", playerInfo.armor);   // 현재 아머량 업데이트

        // _armor는 구매할 아머의 양. 100이 맥스이므로 100- 현재 아머
        int _armor = Mathf.FloorToInt(100 - playerInfo.armor);
        //만약 아머를 구매할 돈이 없다면 
        if (playerInfo.coin < _armor * 5)
        {
            _armor = Mathf.FloorToInt(playerInfo.coin / 5);
        }
        armorPriceText.text = string.Format("{0}", _armor * 5); // 구매 가격 업데이트

        // 만약 아머가 꽉찼을경우 MAX 표시
        if (100 <= playerInfo.armor)
        { armorPriceText.text = string.Format("MAX"); }

        // 버튼 비활성화
        if (100 <= playerInfo.armor || playerInfo.coin < _armor || playerInfo.coin == 0)
        {
            buyArmorBtn.interactable = false;
            buyArmorDisableBtn.SetActive(true);
        }
        else
        {
            buyArmorBtn.interactable = true;
            buyArmorDisableBtn.SetActive(false);
        }

        //  ================================================ 현재 수류탄 업데이트 ================================================
        grenadeValueText.text = string.Format("{0}/5",shooter.grenade);
        grenadePriceText.text = string.Format("300");   // 구매가격 300코인

         // 수류탄이 꽉찼을 경우 MAX
        if (5 <= shooter.grenade)
        { grenadePriceText.text = string.Format("MAX"); }

        // 버튼 비활성화
        if (5 <= shooter.grenade || playerInfo.coin < 300)
        {
            buyGrenadeBtn.interactable = false;
            buyGrenadeDisableBBtn.SetActive(true);
        }
        else
        {
            buyGrenadeBtn.interactable = true;
            buyGrenadeDisableBBtn.SetActive(false);
        }

        //  ================================================ 피스톨 업데이트 ================================================
        pistolValueText.text = string.Format(shooter.tpsPistol.remainingAmmo + "/" + shooter.tpsPistol.maxAmmo);    // 현재 탄 상태 업데이트
        pistolPriceText.text = string.Format("250");   // 구매가격 250코인

        // 피스톨이 꽉찼을 경우 MAX
        if (shooter.tpsPistol.remainingAmmo >= shooter.tpsPistol.maxAmmo)
        { grenadePriceText.text = string.Format("MAX"); }

        // 버튼 비활성화
        if (shooter.tpsPistol.remainingAmmo >= shooter.tpsPistol.maxAmmo || playerInfo.coin < 250)  // 탄이 없거나 돈이 없으면 버튼 비활성화
        {
            buyPistolBtn.interactable = false;
            buyPistolDisableBtn.SetActive(true);
        }
        else
        {
            buyPistolBtn.interactable = true;
            buyPistolDisableBtn.SetActive(false);

        }
        pistolIcon[0].SetActive(true);
        pistolIcon[1].SetActive(false);

        // 피스톨 아이콘 업데이트
        if (shooter.isSMG)
        {
            pistolNameText.text = string.Format("SMG");
            pistolIcon[0].SetActive(false);
            pistolIcon[1].SetActive(true);
        }

        //  ================================================ 라이플 업데이트 ================================================
        rifleValueText.text = string.Format(shooter.tpsRifle.remainingAmmo + "/" + shooter.tpsRifle.maxAmmo);  // 현재 탄 상태 업데이트
        riflePriceText.text = string.Format("250");   // 구매가격 250코인

        // 라이플이 꽉찼을 경우 MAX
        if (shooter.tpsRifle.remainingAmmo >= shooter.tpsRifle.maxAmmo)
        { riflePriceText.text = string.Format("MAX"); }

        // 버튼 비활성화
        if (shooter.tpsRifle.remainingAmmo >= shooter.tpsRifle.maxAmmo || playerInfo.coin < 250 )  // 탄이 없거나 돈이 없으면 버튼 비활성화
        {
            buyRifleBtn.interactable = false;
            buyRifleDisableBtn.SetActive(true);
        }
        else
        {
            buyRifleBtn.interactable = true;
            buyRifleDisableBtn.SetActive(false);

        }

        // 라이플 아이콘 업데이트
        rifleIcon[0].SetActive(false);
        rifleIcon[1].SetActive(false);
        rifleIcon[2].SetActive(false);

        if (shooter.weaponClass == PlayerShooter.WeaponClass.Commando && !shooter.isSCAR)
        {
            rifleNameText.text = string.Format("AR-15 Varmint Rifle");
            rifleIcon[0].SetActive(true);
        }
        else if (shooter.weaponClass == PlayerShooter.WeaponClass.Demolitionist && !shooter.isSCAR)
        {
            rifleNameText.text = string.Format("Grenade Gun");
            rifleIcon[1].SetActive(true);
        }
        if (shooter.isSCAR)
        {
            rifleNameText.text = string.Format("SCAR");
            rifleIcon[2].SetActive(true);
        }


        // SMG 구입 업데이트
        if (playerInfo.coin < 2000 || shooter.isSMG)
        {
            buySmgBtn.interactable = false;
            buySmgDisableBtn.SetActive(true);
            if(shooter.isSMG)
            {
                SmgBtnText.text = string.Format("SOLD");
            }

        }
        else
        {
            buySmgBtn.interactable = true;
            buySmgDisableBtn.SetActive(false);
        }


        // 스카 구입 업데이트
        if (playerInfo.coin < 5000 || shooter.isSCAR)
        {
            buyScarBtn.interactable = false;
            buyScarDisableBtn.SetActive(true);
            if (shooter.isSCAR)
            {
                ScarBtnText.text = string.Format("SOLD");
            }
        }
        else
        {
            buyScarBtn.interactable = true;
            buyScarDisableBtn.SetActive(false);
        }

       

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
        ShopClose();
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
        //MaxAmmo =(int)player.GetComponent<PlayerShooter>().equipedWeapon.maxAmmo;
        //remaining = (int)player.GetComponent<PlayerShooter>().equipedWeapon.remainingAmmo;
        //magazineAmmo = (int)player.GetComponent<PlayerShooter>().equipedWeapon.magazineSize;
                           

        //if(GameManager.instance.wave == waveShop || GameManager.instance.wave == subWaveShop)
        //{
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
        //}
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
                _armor = Mathf.FloorToInt(playerInfo.coin / 5);
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
    public void BuyGrenade()
    {
        if (playerInfo != null)
        {
            // 수류탄이 5개보다 적거나 300코인보다 돈 많을 경우 구매 가능
            if(shooter.grenade < 5 && 300 <= playerInfo.coin)
            {

                shooter.grenade++;

                PlayerUIManager.instance.SetGrenade(shooter.grenade); // 수류탄 개수 추가
                playerInfo.BuyAmmo(300);

            }

        }

    }
    public void BuyPistolAmmo()
    {
        if (playerInfo != null)
        {

            if (250 <= playerInfo.coin && shooter.tpsPistol.remainingAmmo < shooter.tpsPistol.maxAmmo)
            {
                if (shooter.tpsPistol.magazineSize + shooter.tpsPistol.remainingAmmo > shooter.tpsPistol.maxAmmo)
                {
                    shooter.tpsPistol.remainingAmmo = shooter.tpsPistol.maxAmmo;
                    //shooter.GetAmmo(remaining - MaxAmmo);
                }
                else
                {
                    //shooter.GetAmmo(magazineAmmo);
                    shooter.tpsPistol.remainingAmmo += shooter.tpsPistol.magazineSize;
                }
                playerInfo.BuyAmmo(250);
                if(shooter.weaponSlot == 1)
                {
                    PlayerUIManager.instance.totalAmmoText.text = string.Format("{0}", shooter.tpsPistol.remainingAmmo); // UI 총알개수 업데이트
                }
            }
        }
    }
    public void BuyRifleAmmo()
    {
        if (playerInfo != null)
        {

            if (250 <= playerInfo.coin && shooter.tpsRifle.remainingAmmo < shooter.tpsRifle.maxAmmo)
            {
                if (shooter.tpsRifle.magazineSize + shooter.tpsRifle.remainingAmmo > shooter.tpsRifle.maxAmmo)
                {
                    shooter.tpsRifle.remainingAmmo = shooter.tpsRifle.maxAmmo;
                    //shooter.GetAmmo(remaining - MaxAmmo);
                }
                else
                {
                    //shooter.GetAmmo(magazineAmmo);
                    shooter.tpsRifle.remainingAmmo += shooter.tpsRifle.magazineSize;
                }
                playerInfo.BuyAmmo(250);
            }
            if (shooter.weaponSlot == 2)
            {
                PlayerUIManager.instance.totalAmmoText.text = string.Format("{0}", shooter.tpsRifle.remainingAmmo); // UI 총알개수 업데이트
            }
        }
    }

    public void BuyAmmo()
    {
        if (playerInfo != null)
        {
         
            if (playerInfo.coin >= 250 && MaxAmmo > remaining)
            {
                if (magazineAmmo + remaining > MaxAmmo)
                {
                    shooter.GetAmmo(remaining- MaxAmmo);
                }else
                {
                    shooter.GetAmmo(magazineAmmo);

                }
               
                playerInfo.BuyAmmo(250);
            }
        }
    }


    public void BuySMG()
    {
        if (playerInfo != null)
        {
            if (2000 <= playerInfo.coin)
            {
                shooter.BuySMG();
                playerInfo.BuyAmmo(2000);

            }
        }
    }

    public void BuySCAR()
    {
        if (playerInfo != null)
        {
            if (5000 <= playerInfo.coin)
            {
                shooter.BuySCAR();
                playerInfo.BuyAmmo(5000);

            }
        }
    }
}
