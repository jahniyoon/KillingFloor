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

        // ���� ��Ʈ��ũ �ӵ� ����ȭ ����
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        //��ȯ : �÷��̾���� �� ��ũ ���߱�
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region �÷�����
    // �̸��� ���� ���� : '@', '.' �� �־����
    // ��й�ȣ ���� ���� : 6~100 ���� ����
    // �̸� ���� ���� : 3~20 ���� ����
    public void Login()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            GetVirtualCurrencies();                 // ���� Currency ������

            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����
        },
            (error) => Debug.Log("�α��� ����"));
    }

    #region TestLogin
    //[Mijeong]230915 �׽�Ʈ�� �α��� �޼��� �߰�
    public void OnLoginTest01()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test01@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    public void OnLoginTest02()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test02@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    public void OnLoginTest03()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test03@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    public void OnLoginTest04()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test04@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    public void OnLoginTest05()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test05@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    public void OnLoginTest06()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test06@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    public void OnLoginTest07()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test07@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    public void OnLoginTest08()
    {
        // �̸��ϰ� ��й�ȣ ����ؼ� �α��� ��û
        var request = new LoginWithEmailAddressRequest { Email = "test08@test.com", Password = "000000" };

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            // �α��� ������ ����
            GetLeaderboard(result.PlayFabId);       // PlayFab �������� ������
            PhotonNetwork.ConnectUsingSettings();   // Photon ���� ����

            GetVirtualCurrencies();                 // ���� Currency ������
        },
            (error) => Debug.Log("�α��� ����"));
    }
    #endregion

    public void Register()
    {
        // �̸���, ��й�ȣ, ���� �̸����� ��� ��û ����
        var request = new RegisterPlayFabUserRequest
        { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };

        PlayFabClientAPI.RegisterPlayFabUser(request, (result) =>
        {
            Debug.Log("ȸ������ ����");
            SetStat();          // ��� �ʱ�ȭ
            SetData("Lv.0");    // ���� ������ �ʱ�ȭ
        },
            (error) => Debug.Log("ȸ������ ����"));

    }

    // ���� ��� �ʱ�ȭ
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => Debug.Log("�� �������"));
    }

    // PlayFab �������� ���� �������� �޼���
    void GetLeaderboard(string myID)
    {
        // PlayFab ���� ����Ʈ �ʱ�ȭ
        PlayFabUserList.Clear();

        for (int i = 0; i < 10; i++)
        {
            // �������� ���� ������ ��û ����
            var request =
                new GetLeaderboardRequest
                {
                    StartPosition = i * 100,    // ��� ���� ��ġ
                    StatisticName = "IDInfo",   // ��� �̸�
                    MaxResultsCount = 100,      // �ִ� ��� ����
                    ProfileConstraints =
                    new PlayerProfileViewConstraints() { ShowDisplayName = true }   // �÷��̾��� ���÷��� �̸� ǥ��
                };

            // PlayFab ���� �������� ���� ��û ����
            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;

                // �÷��̾� ������ PlayFabUserList�� �߰��ϱ� ���� �������� ��� �ݺ�
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    PlayFabUserList.Add(result.Leaderboard[j]);

                    // �� PlayFab ID�� ã�� MyPlayFabInfo ������ ����
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }
            },
            (error) => { });

        }
    }

    #region ���� ������ ����
    // ���� ������ �����ϴ� �޼���
    void SetData(string curData)
    {
        // ������Ʈ�� ����� ������ ��û ����
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { "HomeLevel", curData } },   // "HomeLevel" Ű�� ���� ������ ����
            Permission = UserDataPermission.Public      // ������ ����
        };

        // PlayFab�� ���� ����� ������ ������Ʈ ��û ����
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => Debug.Log("������ ���� ����"));
    }

    // ���� ������ �������� �޼���
    void GetData(string curID)
    {
        // ����� �����͸� ������ ��û ����
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
        UserRoomDataText.text = "����ID" + curID + "\n" + result.Data["HomeLevel"].Value,
        (error) => Debug.Log("������ �ҷ����� ����"));
    }
    #endregion

    #region ���� Currency
    // ���� Currency �������� �޼���
    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
    }
    // ������ ���� ������ �޼���
    void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        coins = result.VirtualCurrency["CN"];
        stars = result.VirtualCurrency["ST"];

        CoinsValueText.text = "Coins: " + coins.ToString();
        StarsValueText.text = "Stars: " + stars.ToString();

        Debug.Log(result);
        Debug.Log(CoinsValueText.text);
    }
    // ���� �߰� �޼���
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
    //    "�κ� : " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms)
    //    + " / ���� : " + PhotonNetwork.CountOfPlayers;
    //}

    // Photon �������� ����ȭ�� �Ϸ� �� CountOfPlayers�� ������Ʈ�ϵ��� �ڸ�ƾ ���
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
                LobbyInfoText.text = "�κ� : " + newPlayerCount + " / ���� : " + PhotonNetwork.CountOfPlayers;
                currentPlayerCount = newPlayerCount;
            }

            yield return new WaitForSeconds(1f); // 1�ʸ��� ������Ʈ
        }
    }

    public override void OnJoinedLobby()
    {
        // �濡�� �κ�� �� �� �����̾���, �α����ؼ� �κ�� �� �� PlayFabUserList�� ä���� �ð����� ������
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
        if (roomName == "������")
        {
            //PlayFabUserList�� ǥ���̸��� �Է¹��� �г����� ���ٸ� PlayFabID�� Ŀ���� ������Ƽ�� �ְ� ���� �����
            for (int i = 0; i < PlayFabUserList.Count; i++)
            {
                if (PlayFabUserList[i].DisplayName == UserSearchInput.text)
                {
                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.MaxPlayers = 20;
                    roomOptions.CustomRoomProperties = new Hashtable() { { "PlayFabID", PlayFabUserList[i].PlayFabId } };
                    PhotonNetwork.JoinOrCreateRoom(UserSearchInput.text + "���� ����â", roomOptions, null);
                    return;
                }
            }
            print("�г����� ��ġ���� �ʽ��ϴ�");
        }
        else PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 20 }, null);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("�游������");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("����������");



    public override void OnJoinedRoom()
    {
        RoomRenewal();

        string curName = PhotonNetwork.CurrentRoom.Name;
        RoomNameInfoText.text = curName;
        Debug.Log(curName);
        Debug.Log(RoomNameInfoText.text);

        if (curName == "ROOM1" || curName == "ROOM2" || curName == "ROOM3" || curName == "ROOM4") ShowPanel(Room_Panel);

        //�������̸� ������ ��������
        else
        {
            ShowPanel(UserRoom_Panel);

            string curID = PhotonNetwork.CurrentRoom.CustomProperties["PlayFabID"].ToString();
            GetData(curID);

            // ���� �� PlatyFabID Ŀ���� ������Ƽ�� ���� PlayFabID�� ���ٸ� ���� ������ �� ����
            if (curID == MyPlayFabInfo.PlayFabId)
            {
                RoomNameInfoText.text += " (���� ����â)";

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
        (error) => { Debug.Log("���� �ҷ����� ����"); }
        );

        //UserNickNameText.text += PhotonNetwork.PlayerList[i].NickName + "\n" + result.Data["HomeLevel"].Value;
        RoomNumInfoText.text = PhotonNetwork.CurrentRoom.PlayerCount + "�� / " + PhotonNetwork.CurrentRoom.MaxPlayers + "�ִ� �ο�";
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
        // �ڱ��ڽ��� �濡���� �� ������ �����ϰ�, �� ���� �� 1�� �ڿ� �� �ҷ�����
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