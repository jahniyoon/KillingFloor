using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using System;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject Lang_Panel, Room_Panel, UserRoom_Panel, Lobby_Panel, Login_Panel, Class_Panel, Store_Panel, Lobby_Screen, Option_Panel, PlayerProfile;

    [Header("Player")]
    public string localPlayerName = default;
    public string localPlayerLv = default;
    public string localPlayerClass = default;
    public int localCashItem = default;

    [Header("Login")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();
    public InputField EmailInput, PasswordInput, UsernameInput, LoginIDInput, LoginPWInput;
    public GameObject RegistBox;
    public GameObject LoginBox;
    public Text RegistInfoText;
    public Text LoginInfoText;

    [Header("Lobby")]
    public InputField UserSearchInput;
    public Text LobbyInfoText, UserNickNameText, UserNameText;

    [Header("Room")]
    public InputField SetDataInput;
    public GameObject SetDataBtnObj, MasterStartBtn, OtherReadyBtn;
    public Text UserRoomDataText, RoomNameInfoText, RoomNumInfoText;
    public Button MasterStartButton, OtherReadyButton;
    public GameObject masterStartBtnDisable;

    [Header("Store")]
    public Text CoinsValueText;
    public Text StarsValueText;
    public int coins = default;
    public int stars = default;
    public GameObject playerSample;

    public ItemToBuy[] Items;
    public GameObject ContentArea;

    public GameObject ItemObj;
    public GameObject InventoryContent;
    public enum State { Login, Lobby, Room, Class, Store, Option };
    [Header("Lobby UI")]
    public State state;
    public Image[] buttonBackGround;    // 로비 버튼 선택여부
    public PlayerProfile[] playerInfo;

    bool isLoaded;
    int readyCheck = -1;
    int readyCount = 0;


    public static NetworkManager net_instance;
    void Awake()
    {
        if (net_instance == null)
        {
            net_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (net_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //지환 : 플레이어들의 씬 씽크 맞추기
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        StartCoroutine(UpdatePlayerCount());
    }

    #region 플레이팹
    // 이메일 충족 조건 : '@', '.' 이 있어야함
    // 비밀번호 충족 조건 : 6~100 자의 문자
    // 이름 충족 조건 : 3~20 자의 문자
    public void Login()
    {
        LoginInfoText.text = "Loading...";
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = LoginIDInput.text, Password = LoginPWInput.text };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            LoginInfoText.text = "접속중...";

            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) =>
            {
                Debug.Log("로그인 실패");
                LoginInfoText.text = "로그인 실패";

            });
    }
    public void Register()
    {
        RegistInfoText.text = "Loading...";
        // 이메일, 비밀번호, 유저 이름으로 등록 요청 생성
        var request = new RegisterPlayFabUserRequest
        { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };

        PlayFabClientAPI.RegisterPlayFabUser(request, (result) =>
        {
            Debug.Log("회원가입 성공");
            RegistInfoText.text = "회원가입 성공";
            SetStat();          // 통계 초기화
            SetData("0");    // 유저 데이터 초기화
            SetClass("Commando");
        },
            (error) =>
            {
                Debug.Log("회원가입 실패");
                RegistInfoText.text = "회원가입 실패";

            });

    }

    #region TestLogin
    //[Mijeong]230915 테스트용 로그인 메서드 추가
    public void OnLoginTest01()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test01@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    public void OnLoginTest02()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test02@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    public void OnLoginTest03()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test03@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    public void OnLoginTest04()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test04@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    public void OnLoginTest05()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test05@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    public void OnLoginTest06()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test06@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    public void OnLoginTest07()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test07@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    public void OnLoginTest08()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = "test08@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴
            SetLocalPlayerData();                   // 로컬플레이어데이터 세팅
            GetItemPrices();                        // 아이템 가격 가져옴
            UpdateInventory();                      // 유저 인벤토리 아이템 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
    }
    #endregion


    // 유저 통계 초기화
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => Debug.Log("값 저장실패"));
    }

    // PlayFab 리더보드 정보 가져오는 메서드
    void GetLeaderboard(string myID)
    {
        // PlayFab 유저 리스트 초기화
        PlayFabUserList.Clear();

        for (int i = 0; i < 10; i++)
        {
            // 리더보드 정보 가져올 요청 생성
            var request =
                new GetLeaderboardRequest
                {
                    StartPosition = i * 100,    // 결과 시작 위치
                    StatisticName = "IDInfo",   // 통계 이름
                    MaxResultsCount = 100,      // 최대 결과 개수
                    ProfileConstraints =
                    new PlayerProfileViewConstraints() { ShowDisplayName = true }   // 플레이어의 디스플레이 이름 표시
                };

            // PlayFab 통해 리더보드 정보 요청 전송
            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;

                // 플레이어 정보를 PlayFabUserList에 추가하기 위해 리더보드 결과 반복
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    PlayFabUserList.Add(result.Leaderboard[j]);

                    // 내 PlayFab ID를 찾아 MyPlayFabInfo 변수에 저장
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }
            },
            (error) => { });

        }
    }

    #region 유저 데이터 설정
    // 유저 데이터 설정하는 메서드
    public void SetData(string curData)
    {
        // 업데이트할 사용자 데이터 요청 생성
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { "HomeLevel", curData } },   // "HomeLevel" 키를 가진 데이터 설정
            Permission = UserDataPermission.Public     // 데이터 공개
        };

        // PlayFab를 통해 사용자 데이터 업데이트 요청 전송
        PlayFabClientAPI.UpdateUserData(request, (result) =>
        {
            Debug.Log("SetData 성공 -> " + result);
            Debug.Log($"curData : {curData}");

            localPlayerLv = curData;
        },
        (error) => Debug.Log("데이터 저장 실패"));
    }

    public void SetClass(string curData)
    {
        // 업데이트할 사용자 데이터 요청 생성
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { "Class", curData } },   // "HomeLevel" 키를 가진 데이터 설정
            Permission = UserDataPermission.Public     // 데이터 공개
        };

        // PlayFab를 통해 사용자 데이터 업데이트 요청 전송
        PlayFabClientAPI.UpdateUserData(request, (result) =>
        {
            Debug.Log("SetData 성공 -> " + result);
            Debug.Log($"curData : {curData}");

            localPlayerClass = curData;
        },
        (error) => Debug.Log("데이터 저장 실패"));
    }

    //[MiJeong] 231001 로컬플레이어 값 Null 오류 수정
    public void SetLocalPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = MyPlayFabInfo.PlayFabId }, (result) =>
        {
            if (MyPlayFabInfo.DisplayName != null && MyPlayFabInfo.DisplayName != localPlayerName)
            {
                PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;

                localPlayerName = MyPlayFabInfo.DisplayName;
                localPlayerLv = result.Data["HomeLevel"].Value;
                localPlayerClass = result.Data["Class"].Value;
                UserNameText.text = "Name: " + localPlayerName + "\nLevel: " + localPlayerLv;

                playerInfo[0].nickName.text = localPlayerName;
                playerInfo[0].level.text = localPlayerLv;
                playerInfo[0].className.text = localPlayerClass;
                SetClassIcon(0, result.Data["Class"].Value);
            }
            else
            {
                SetLocalPlayerData();
            }
        },
            (error) => Debug.Log("데이터 불러오기 실패" + error.ErrorMessage));
    }
    #endregion
    #endregion

    #region 유저 Currency
    // 유저 Currency 가져오는 메서드
    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
    }
    // 아이템 구매 성공시 메서드
    void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        coins = result.VirtualCurrency["CN"];
        stars = result.VirtualCurrency["GD"];

        CoinsValueText.text = "Coins: " + coins.ToString();
        StarsValueText.text = "Golds: " + stars.ToString();

        Debug.Log(result);
        Debug.Log(CoinsValueText.text);
    }
    // 코인 추가 요청 메서드
    public void OnAddVirtualCurrency(/*int addCoin*/)
    {
        int addCoin = 50;

        var request = new AddUserVirtualCurrencyRequest { VirtualCurrency = "CN", Amount = addCoin };
        PlayFabClientAPI.AddUserVirtualCurrency(request, GrantVirtualCurrencySuccess, OnError);
    }
    // 코인 추가 요청 성공시 콜백
    void GrantVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Add Coins Granted !");

        GetVirtualCurrencies();
        //coins += 50;
        //CoinsValueText.text = "Coins: " + coins.ToString();
        //StarsValueText.text = "Stars: " + stars.ToString();
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
    #endregion

    #region Store_Panel
    public void StoreBtn()
    {
        Store_Panel.SetActive(true);
    }
    public void ExitStore_Panel()
    {
        Store_Panel.SetActive(false);
    }

    public void GetItemPrices()
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion = "Player Items Catalog";
        PlayFabClientAPI.GetCatalogItems(request, result =>
        {
            List<CatalogItem> items = result.Catalog;
            foreach (CatalogItem item in items)
            {
                uint cost = item.VirtualCurrencyPrices["CN"];

                foreach (ItemToBuy editorItems in Items)
                {
                    if (editorItems.Name == item.ItemId)
                    {
                        editorItems.Cost = (int)cost;
                    }
                }
            }

            foreach (ItemToBuy i in Items)
            {

                GameObject itemObject = Instantiate(ItemObj, ContentArea.transform.position, Quaternion.identity);
                itemObject.transform.GetChild(1).GetComponent<Text>().text = i.Name;
                itemObject.transform.GetChild(2).GetComponent<Text>().text = "[" + i.Cost + " Coin]";
                itemObject.GetComponent<Image>().sprite = i.GetComponent<Image>().sprite;
                itemObject.GetComponent<Image>().preserveAspect = true;     // 이미지 종횡비 유지하도록 설정

                itemObject.transform.SetParent(ContentArea.transform);
                itemObject.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { MakePurchase(i.Name, i.Cost); });
                itemObject.transform.localScale = Vector3.one;  // 지환추가
                itemObject.transform.SetParent(ContentArea.transform);
            }

        },
        error => { });
    }

    void UpdateInventory()
    {
        // InventoryContent의 모든 자식 오브젝트 삭제
        foreach (Transform child in InventoryContent.transform)
        {
            Destroy(child.gameObject);
        }

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            List<ItemInstance> itemIv = result.Inventory;
            foreach (ItemInstance i in itemIv)
            {
                foreach (ItemToBuy editorI in Items)
                {
                    if (editorI.Name == i.ItemId)
                    {
                        GameObject itemObject = Instantiate(ItemObj, InventoryContent.transform.position, Quaternion.identity);
                        itemObject.transform.GetChild(1).GetComponent<Text>().text = i.ItemId;
                        itemObject.transform.GetChild(2).GetComponent<Text>().text = "[" + editorI.Cost + " Coin]";
                        itemObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "EQUIP";
                        itemObject.GetComponent<Image>().sprite = editorI.GetComponent<Image>().sprite;
                        itemObject.GetComponent<Image>().preserveAspect = true;     // 이미지 종횡비 유지하도록 설정

                        itemObject.transform.SetParent(InventoryContent.transform);
                        itemObject.transform.localScale = Vector3.one;

                        //[MiJeong] 231002: 상점에서 EQUIP 값 전달하기 위해
                        itemObject.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { PlayerColorChange(i.ItemId); });


                    }

                }
            }
        }, error => { });
    }

    void MakePurchase(string name, int price)
    {
        PurchaseItemRequest request = new PurchaseItemRequest();
        request.CatalogVersion = "Player Items Catalog";
        request.ItemId = name;
        request.Price = price;
        request.VirtualCurrency = "CN";

        PlayFabClientAPI.PurchaseItem(request, result =>
        {
            Debug.Log("구매 성공");
            //UpdateInventory();

            foreach (ItemToBuy editorItems in Items)
            {
                if (editorItems.Name == name)
                {
                    GameObject itemObject = Instantiate(ItemObj, InventoryContent.transform.position, Quaternion.identity);
                    itemObject.transform.GetChild(1).GetComponent<Text>().text = name;
                    itemObject.transform.GetChild(2).GetComponent<Text>().text = "[" + price + " Coin]";
                    itemObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "EQUIP";
                    itemObject.GetComponent<Image>().sprite = editorItems.GetComponent<Image>().sprite;
                    itemObject.GetComponent<Image>().preserveAspect = true;     // 이미지 종횡비 유지하도록 설정

                    itemObject.transform.SetParent(InventoryContent.transform);
                    itemObject.transform.localScale = Vector3.one;
                    //[MiJeong] 231002: 상점에서 EQUIP 값 전달하기 위해
                    itemObject.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { PlayerColorChange(name); });

                }
            }


            GetVirtualCurrencies();
        },
            error => { Debug.Log(error.ErrorMessage); });
    }

    //[Mijeong] 231002: 플레이어 스킨 선택한 값 전달
    public void PlayerColorChange(string colorName)
    {

        switch (colorName)
        {
            case "Default":
                localCashItem = 0;
                break;

            case "SWAT":
                localCashItem = 1;
                break;

            case "GreenArmyMan":
                localCashItem = 2;
                break;
            case "GoldMan":
                localCashItem = 3;
                break;
        }

        playerSample.GetComponent<PlayerShop>().EquipMat(localCashItem);
        Debug.Log(colorName);
    }
    #endregion

    #region Lang_Panel
    public void LangBtn()
    {
        Lang_Panel.SetActive(true);
    }
    public void ExitLang_Panel()
    {
        Lang_Panel.SetActive(false);
    }
    #endregion

    #region Lobby
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    // Photon 서버와의 동기화가 완료 후 CountOfPlayers를 업데이트하도록 코르틴 사용
    private int currentPlayerCount = 0;

    private IEnumerator UpdatePlayerCount()
    {
        while (true)
        {
            int newPlayerCount = PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms;

            if (newPlayerCount != currentPlayerCount)
            {
                LobbyInfoText.text = "로비 : " + newPlayerCount + " / 접속 : " + PhotonNetwork.CountOfPlayers;
                currentPlayerCount = newPlayerCount;
            }

            yield return new WaitForSeconds(1f); // 1초마다 업데이트
        }
    }

    public override void OnJoinedLobby()
    {
        // 방에서 로비로 올 땐 딜레이없고, 로그인해서 로비로 올 땐 PlayFabUserList가 채워질 시간동안 딜레이
        if (isLoaded)
        {
            SetLocalPlayerProfile();
            ShowPanel(Lobby_Panel);
            ShowUserNickName();
        }
        else Invoke("OnJoinedLobbyDelay", 1);
    }

    // 로컬 플레이어 닉네임 할당
    void OnJoinedLobbyDelay()
    {
        isLoaded = true;

        SetLocalPlayerProfile();
        LobbyScreen(true);      // 로비 배경영상를 켠다.
        state = State.Lobby;    // 상태 로비로 변경
        PlayerProfile.gameObject.SetActive(true);

        ShowPanel(Lobby_Panel);
        ShowUserNickName();
    }

    public void SetLocalPlayerProfile()
    {
        //PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;

        //Debug.Log($"포톤네트워크 닉네임 : {PhotonNetwork.LocalPlayer.NickName}");
        //playerInfo[0].nickName.text = string.Format(PhotonNetwork.LocalPlayer.NickName);
        //Debug.Log($"playerInfo[0].nickName: {playerInfo[0].nickName.text}");
        //playerInfo[0].level.text = string.Format(localPlayerLv);
        //playerInfo[0].className.text = localPlayerClass;

        if (playerInfo[0].nickName == null)
        {
            Debug.Log("로컬 플레이어 데이터가 초기화 되어서 다시 불러옴");
            SetLocalPlayerData();
        }

        SetClassIcon(0, localPlayerClass);
    }
    void ShowPanel(GameObject curPanel)
    {

        Room_Panel.SetActive(false);
        UserRoom_Panel.SetActive(false);
        Lobby_Panel.SetActive(false);
        Login_Panel.SetActive(false);
        Store_Panel.SetActive(false);

        curPanel.SetActive(true);
    }

    void ShowUserNickName()
    {
        UserNickNameText.text = "";
        for (int i = 0; i < PlayFabUserList.Count; i++)
        {
            UserNickNameText.text += PlayFabUserList[i].DisplayName + "\n";
        }
    }

    public void XBtn()
    {
        if (PhotonNetwork.InLobby) PhotonNetwork.Disconnect();
        else if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyScreen(false);// 로비를 끈다
        isLoaded = false;
        ShowPanel(Login_Panel);
    }
    #endregion

    //[Mijeong] 230927 불필요한 코드 삭제
    #region UserRoom
    public void JoinOrCreateRoom(string roomName)
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 6 }, null);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");


    //[Mijeong] 230927 불필요한 코드 삭제
    public override void OnJoinedRoom()
    {
        RoomRenewal();

        // [Mijeong] 230925 : 게임 준비,시작 버튼 활성화 메서드 추가
        PlayerReadyBtn();

        string curName = PhotonNetwork.CurrentRoom.Name;
        RoomNameInfoText.text = curName;
        Debug.Log(curName);

        if (curName == "ROOM1" || curName == "ROOM2" || curName == "ROOM3" || curName == "ROOM4")
        {
            state = State.Room;
            ShowPanel(Room_Panel);
        }
        else { };
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) => RoomRenewal();

    public override void OnPlayerLeftRoom(Player otherPlayer) => RoomRenewal();

    void RoomRenewal()
    {

        UserNickNameText.text = "";
        ResetPlayerUI();

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            // 방 안의 UI 세팅
            int playerIndex = i + 1; // 현재 플레이어 인덱스 저장

            playerInfo[playerIndex].gameObject.SetActive(true);
            playerInfo[playerIndex].nickName.text = PhotonNetwork.PlayerList[i].NickName;

            for (int j = 0; j < PlayFabUserList.Count; j++)
            {
                if (PhotonNetwork.PlayerList[i].NickName == PlayFabUserList[j].DisplayName)
                {
                    int currentPlayerIndex = playerIndex; // 클로저에서 사용할 현재 플레이어 인덱스

                    PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = PlayFabUserList[j].PlayFabId }, (result) =>
                    {
                        if (result.Data.ContainsKey("HomeLevel"))
                        {
                            string levelValue = result.Data["HomeLevel"].Value;
                            playerInfo[currentPlayerIndex].level.text = levelValue;
                            playerInfo[currentPlayerIndex].className.text = result.Data["Class"].Value;
                            SetClassIcon(currentPlayerIndex, result.Data["Class"].Value);

                            Debug.Log($"Player {currentPlayerIndex}의 레벨: {levelValue}");
                        }
                        else
                        {
                            Debug.Log($"Player {currentPlayerIndex}의 레벨 정보가 없습니다.");
                        }
                    },
                    (error) => Debug.Log("레벨 불러오지 못함"));
                }
            }
        }

        RoomNumInfoText.text = PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대 인원";
    }
    void ResetPlayerUI()
    {
        for (int i = 0; i < 6; i++)
        { playerInfo[i + 1].gameObject.SetActive(false); }
    }
    public override void OnLeftRoom()
    {
        SetDataInput.gameObject.SetActive(false);
        SetDataBtnObj.SetActive(false);

        SetDataInput.text = "";
        UserSearchInput.text = "";
        UserRoomDataText.text = "";
    }

    //public void SetDataBtn()
    //{
    //    // 자기자신의 방에서만 값 저장이 가능하고, 값 저장 후 1초 뒤에 값 불러오기
    //    SetData(SetDataInput.text);
    //    Invoke("SetDataBtnDelay", 1);
    //}

    //void SetDataBtnDelay() => GetData(PhotonNetwork.CurrentRoom.CustomProperties["PlayFabID"].ToString());
    #endregion

    // [Mijeong] 230925 : 게임 준비,시작 체크 메서드 추가
    #region Player Ready Check
    void PlayerReadyBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MasterStartBtn.SetActive(true); // 룸 마스터만 스타트 버튼 활성화
            OtherReadyBtn.SetActive(false);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                masterStartBtnDisable.gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    public void OnStartCheck(int readyCheck, int index)
    {
        if(readyCheck == -1)
        {
            playerInfo[index].ready.gameObject.SetActive(false);
        }
        else if (readyCheck == 1)
        {
            playerInfo[index].ready.gameObject.SetActive(true);
        }

        // 아래는 마스터클라이언트가 아니면 리턴
        if(!PhotonNetwork.IsMasterClient)
        { return; }
        photonView.RPC("OnStartCheck", RpcTarget.Others, readyCheck, index);


        readyCount += readyCheck;
        Debug.Log($"readyCount: {readyCount}");
        Debug.Log($"readyCheck: {readyCheck}");

        if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            MasterStartButton.interactable = true;
            masterStartBtnDisable.gameObject.SetActive(false);
            Debug.Log("모든 클라이언트 준비 완료");
        }
        else
        {
            MasterStartButton.interactable = false;
            masterStartBtnDisable.gameObject.SetActive(true);

            Debug.Log("모든 클라이언트가 준비하지 않았습니다.");
        }
    }

    public void OnReadyCheck()
    {
        int profileIndex = 0; // 레디로 변경할 프로필의 넘버

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            int playerIndex = i + 1; // 현재 플레이어 인덱스 저장

            if (PhotonNetwork.PlayerList[i].NickName == localPlayerName)
            {
                profileIndex = playerIndex;
            }

        }


        switch (readyCheck)
        {
            case -1:
                readyCheck = 1;
                photonView.RPC("OnStartCheck", RpcTarget.MasterClient, readyCheck, profileIndex);
                break;

            case 1:
                readyCheck = -1;
                photonView.RPC("OnStartCheck", RpcTarget.MasterClient, readyCheck, profileIndex);
                OtherReadyBtn.GetComponent<Button>().interactable = false;
                OtherReadyBtn.GetComponent<Button>().interactable = true;
                break;
        }
    }
    #endregion

    #region PlayScene Load
    public void OnPlayerTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("PlayerTestScene");
        }
    }
    public void OnGunTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GunTestScene");
        }
    }
    public void OnZombieTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("ZombieTestScene");
        }
    }
    public void OnMainTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Main");
        }
    }
    public void OnLevelTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("LevelTestScene");
        }
    }
    #endregion

    // 지환작성
    #region LobbyUI

    // 지환
    void LobbyScreen(bool isLobby)
    {
        Lobby_Screen.SetActive(isLobby);
    }


    public void HomeButton()
    {
        if (state == State.Lobby || state == State.Room)
        {
            return;
        }

        // 로비면 로비 상태로, 방에 있으면 방으로 상태 변경
        if (PhotonNetwork.InLobby) { state = State.Lobby; }
        else if (PhotonNetwork.InRoom)
        {
            state = State.Room;
            RoomRenewal();

        }

        SetButtonColor(0);
        SetPanel();
    }
    // 방 나가기 버튼
    public void LeaveRoomButton()
    {
        state = State.Lobby;
        SetPanel();
        PhotonNetwork.LeaveRoom();
    }
    public void ClassButton()
    {
        if (state == State.Class)
        {
            return;
        }
        state = State.Class;

        SetButtonColor(3);
        SetPanel();
    }
    // 상점 버튼
    public void StoreButton()
    {
        if (state == State.Store)
        {
            return;
        }
        state = State.Store;

        SetButtonColor(1);
        SetPanel();
    }
    public void OptionButton()
    {
        if (state == State.Option)
        {
            return;
        }
        state = State.Option;

        SetButtonColor(2);
        SetPanel();
    }
    public void ExitButton()
    {
        state = State.Login;
        LobbyScreen(false);
        PhotonNetwork.Disconnect();

        SetButtonColor(0);
        SetPanel();
    }
    public void RegistButton()
    {
        RegistBox.gameObject.SetActive(!RegistBox.gameObject.activeSelf);
        LoginBox.gameObject.SetActive(!RegistBox.gameObject.activeSelf);
    }


    // 버튼 배경의 색을 변경해주는 메서드
    public void SetButtonColor(int num)
    {
        buttonBackGround[0].color = new Color(255, 255, 255, 0);    // Home
        buttonBackGround[1].color = new Color(255, 255, 255, 0);    // Store
        buttonBackGround[2].color = new Color(255, 255, 255, 0);    // Option
        buttonBackGround[3].color = new Color(255, 255, 255, 0);    // Class
                                                                    // 요청한 버튼의 색만 켜주기
        buttonBackGround[num].color = new Color(255, 255, 255, 0.8f);

    }
    // 상태에 따라 패널 변경 메서드
    public void SetPanel()
    {
        Login_Panel.SetActive(false);
        Lobby_Panel.SetActive(false);
        Room_Panel.SetActive(false);
        Class_Panel.SetActive(false);
        UserRoom_Panel.SetActive(false);
        Option_Panel.SetActive(false);
        Store_Panel.SetActive(false);
        PlayerProfile.SetActive(false);

        playerSample.SetActive(false);


        switch (state)
        {
            case State.Lobby:
                Lobby_Panel.SetActive(true);
                PlayerProfile.SetActive(true);
                break;
            case State.Room:
                Room_Panel.SetActive(true);
                PlayerProfile.SetActive(false);
                break;
            case State.Store:
                Store_Panel.SetActive(true);
                PlayerProfile.SetActive(false);
                playerSample.SetActive(true);

                break;
            case State.Option:
                Option_Panel.SetActive(true);
                PlayerProfile.SetActive(false);
                break;
            case State.Login:
                Login_Panel.SetActive(true);
                PlayerProfile.SetActive(true);
                break;
            case State.Class:
                Class_Panel.SetActive(true);
                PlayerProfile.SetActive(true);
                break;
        }
    }

    #endregion

    #region SetClass
    public void ClassChangeCommando()
    {
        NetworkManager.net_instance.localPlayerClass = "Commando";
        Debug.Log(NetworkManager.net_instance.localPlayerClass + "클래스 변경");
        // 데이터에 변경된 레벨 서버에 저장하기 위해서 꼭 필요
        NetworkManager.net_instance.SetClass(NetworkManager.net_instance.localPlayerClass);

        playerInfo[0].className.text = NetworkManager.net_instance.localPlayerClass;
        SetClassIcon(0, "Commando");
        SetClass("Commando");
        SetLocalPlayerData();

        photonView.RPC("SyncClass", RpcTarget.All);

    }
    public void ClassChangeDemolitionist()
    {
        NetworkManager.net_instance.localPlayerClass = "Demolitionist";
        Debug.Log(NetworkManager.net_instance.localPlayerClass + "클래스 변경");
        // 데이터에 변경된 레벨 서버에 저장하기 위해서 꼭 필요
        NetworkManager.net_instance.SetClass(NetworkManager.net_instance.localPlayerClass);

        playerInfo[0].className.text = NetworkManager.net_instance.localPlayerClass;
        SetClassIcon(0, "Demolitionist");
        SetClass("Demolitionist");
        SetLocalPlayerData();

        photonView.RPC("SyncClass", RpcTarget.All);

    }

    public void SetClassIcon(int index, string playerClass)
    {
        playerInfo[index].classIcon[0].SetActive(false);    // 코만도
        playerInfo[index].classIcon[1].SetActive(false);    // 데몰리셔니스트

        switch (playerClass)
        {
            case "Commando":
                playerInfo[index].classIcon[0].SetActive(true);
                break;
            case "Demolitionist":
                playerInfo[index].classIcon[1].SetActive(true);
                break;
        }
    }

    [PunRPC]
    public void SyncClass()
    {
        RoomRenewal();
    }
    #endregion

}