using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    public GameObject Lang_Panel, Room_Panel, UserRoom_Panel, Lobby_Panel, Login_Panel, Store_Panel;

    [Header("Login")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();
    public InputField EmailInput, PasswordInput, UsernameInput;

    [Header("Lobby")]
    public InputField UserSearchInput;
    public Text LobbyInfoText, UserNickNameText;

    [Header("Room")]
    public InputField SetDataInput;
    public GameObject SetDataBtnObj;
    public Text UserRoomDataText, RoomNameInfoText, RoomNumInfoText;

    [Header("Store")]
    public Text CoinsValueText;
    public Text StarsValueText;

    bool isLoaded;

    public int coins = default;
    public int stars = default;

    void Awake()
    {
        instance = this;

        // 포톤 네트워크 속도 최적화 설정
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        //지환 : 플레이어들의 씬 씽크 맞추기
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region 플레이팹
    // 이메일 충족 조건 : '@', '.' 이 있어야함
    // 비밀번호 충족 조건 : 6~100 자의 문자
    // 이름 충족 조건 : 3~20 자의 문자
    public void Login()
    {
        // 이메일과 비밀번호 사용해서 로그인 요청
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // 로그인 성공시 실행
            GetLeaderboard(result.PlayFabId);       // PlayFab 리더보드 가져옴
            GetVirtualCurrencies();                 // 유저 Currency 가져옴

            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결
        },
            (error) => Debug.Log("로그인 실패"));
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
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
            PhotonNetwork.ConnectUsingSettings();   // Photon 서버 연결

            GetVirtualCurrencies();                 // 유저 Currency 가져옴
        },
            (error) => Debug.Log("로그인 실패"));
    }
    #endregion

    public void Register()
    {
        // 이메일, 비밀번호, 유저 이름으로 등록 요청 생성
        var request = new RegisterPlayFabUserRequest
        { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };

        PlayFabClientAPI.RegisterPlayFabUser(request, (result) =>
        {
            Debug.Log("회원가입 성공");
            SetStat();          // 통계 초기화
            SetData("Lv.0");    // 유저 데이터 초기화
        },
            (error) => Debug.Log("회원가입 실패"));

    }

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
    void SetData(string curData)
    {
        // 업데이트할 사용자 데이터 요청 생성
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { "HomeLevel", curData } },   // "HomeLevel" 키를 가진 데이터 설정
            Permission = UserDataPermission.Public      // 데이터 공개
        };

        // PlayFab를 통해 사용자 데이터 업데이트 요청 전송
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => Debug.Log("데이터 저장 실패"));
    }

    // 유저 데이터 가져오는 메서드
    void GetData(string curID)
    {
        // 사용자 데이터를 가져올 요청 생성
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
        UserRoomDataText.text = "고유ID" + curID + "\n" + result.Data["HomeLevel"].Value,
        (error) => Debug.Log("데이터 불러오기 실패"));
    }
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
        stars = result.VirtualCurrency["ST"];

        CoinsValueText.text = "Coins: " + coins.ToString();
        StarsValueText.text = "Stars: " + stars.ToString();

        Debug.Log(result);
        Debug.Log(CoinsValueText.text);
    }
    // 코인 추가 메서드
    public void GrantVirtualCurrency()
    {
        var request = new AddUserVirtualCurrencyRequest { VirtualCurrency = "CN", Amount = 50 };
        PlayFabClientAPI.AddUserVirtualCurrency(request, OnGrantVirtualCurrencySuccess, OnError);
    }
    void OnGrantVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Add 50 Coins Granted !");

        coins += 50;
        CoinsValueText.text = "Coins: " + coins.ToString();
        StarsValueText.text = "Stars: " + stars.ToString();
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
    #endregion

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

    //LEGACY:
    //void Update()
    //{
    //    LobbyInfoText.text =
    //    "로비 : " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms)
    //    + " / 접속 : " + PhotonNetwork.CountOfPlayers;
    //}

    // Photon 서버와의 동기화가 완료 후 CountOfPlayers를 업데이트하도록 코르틴 사용
    private int currentPlayerCount = 0;

    void Start()
    {
        StartCoroutine(UpdatePlayerCount());
    }

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
            ShowPanel(Lobby_Panel);
            ShowUserNickName();
        }
        else Invoke("OnJoinedLobbyDelay", 3);
    }
    void OnJoinedLobbyDelay()
    {
        isLoaded = true;
        PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;

        ShowPanel(Lobby_Panel);
        ShowUserNickName();
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
        isLoaded = false;
        ShowPanel(Login_Panel);
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
    #endregion

    #region UserRoom
    public void JoinOrCreateRoom(string roomName)
    {
        if (roomName == "유저방")
        {
            //PlayFabUserList의 표시이름과 입력받은 닉네임이 같다면 PlayFabID를 커스텀 프로퍼티로 넣고 방을 만든다
            for (int i = 0; i < PlayFabUserList.Count; i++)
            {
                if (PlayFabUserList[i].DisplayName == UserSearchInput.text)
                {
                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.MaxPlayers = 20;
                    roomOptions.CustomRoomProperties = new Hashtable() { { "PlayFabID", PlayFabUserList[i].PlayFabId } };
                    PhotonNetwork.JoinOrCreateRoom(UserSearchInput.text + "님의 정보창", roomOptions, null);
                    return;
                }
            }
            print("닉네임이 일치하지 않습니다");
        }
        else PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 20 }, null);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");



    public override void OnJoinedRoom()
    {
        RoomRenewal();

        string curName = PhotonNetwork.CurrentRoom.Name;
        RoomNameInfoText.text = curName;
        Debug.Log(curName);
        Debug.Log(RoomNameInfoText.text);

        if (curName == "ROOM1" || curName == "ROOM2" || curName == "ROOM3" || curName == "ROOM4") ShowPanel(Room_Panel);

        //유저방이면 데이터 가져오기
        else
        {
            ShowPanel(UserRoom_Panel);

            string curID = PhotonNetwork.CurrentRoom.CustomProperties["PlayFabID"].ToString();
            GetData(curID);

            // 현재 방 PlatyFabID 커스텀 프로퍼티가 나의 PlayFabID와 같다면 값을 저장할 수 있음
            if (curID == MyPlayFabInfo.PlayFabId)
            {
                RoomNameInfoText.text += " (나의 정보창)";

                SetDataInput.gameObject.SetActive(true);
                SetDataBtnObj.SetActive(true);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) => RoomRenewal();

    public override void OnPlayerLeftRoom(Player otherPlayer) => RoomRenewal();

    void RoomRenewal()
    {
        UserNickNameText.text = "";

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                UserNickNameText.text += PhotonNetwork.PlayerList[i].NickName + " : " + result.Data["HomeLevel"].Value + "\n";
            }
        },
        (error) => { Debug.Log("레벨 불러오지 못함"); }
        );

        //UserNickNameText.text += PhotonNetwork.PlayerList[i].NickName + "\n" + result.Data["HomeLevel"].Value;
        RoomNumInfoText.text = PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대 인원";
    }

    public override void OnLeftRoom()
    {
        SetDataInput.gameObject.SetActive(false);
        SetDataBtnObj.SetActive(false);

        SetDataInput.text = "";
        UserSearchInput.text = "";
        UserRoomDataText.text = "";
    }

    public void SetDataBtn()
    {
        // 자기자신의 방에서만 값 저장이 가능하고, 값 저장 후 1초 뒤에 값 불러오기
        SetData(SetDataInput.text);
        Invoke("SetDataBtnDelay", 1);
    }

    void SetDataBtnDelay() => GetData(PhotonNetwork.CurrentRoom.CustomProperties["PlayFabID"].ToString());
    #endregion

    #region PlayScene Load
    public void OnPlayerTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("PlayerTestScene");
        }

        Debug.Log(UserNickNameText.text);
    }
    public void OnGunTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GunTestScene");
        }

        Debug.Log(UserNickNameText.text);
    }
    public void OnZombieTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("ZombieTestScene");
        }

        Debug.Log(UserNickNameText.text);
    }
    public void OnMainTestScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Main");
        }

        Debug.Log(UserNickNameText.text);
    }
    #endregion
}