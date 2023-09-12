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
    public GameObject Lang_Panel, Room_Panel, UserRoom_Panel, Lobby_Panel, Login_Panel;

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

    bool isLoaded;

    void Awake()
    {
        // ���� ����ȭ
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    #region �÷�����
    // �̸��� ���� ���� : '@', '.' �� �־����
    // ��й�ȣ ���� ���� : 6~100 ���� ����
    // �̸� ���� ���� : 3~20 ���� ����
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        //PlayFabClientAPI.LoginWithEmailAddress(request, (result) => Debug.Log("�α��� ����"), (error) => Debug.Log("�α��� ����"));
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetLeaderboard(result.PlayFabId); PhotonNetwork.ConnectUsingSettings(); },
            (error) => Debug.Log("�α��� ����"));

    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest
        { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };
        //PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Debug.Log("ȸ������ ����"), (error) => Debug.Log("ȸ������ ����"));
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { Debug.Log("ȸ������ ����"); SetStat(); SetData("default"); },
            (error) => Debug.Log("ȸ������ ����"));

    }
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => Debug.Log("�� �������"));
    }
    void GetLeaderboard(string myID)
    {
        PlayFabUserList.Clear();

        for (int i = 0; i < 10; i++)
        {
            var request =
                new GetLeaderboardRequest
                {
                    StartPosition = i * 100,
                    StatisticName = "IDInfo",
                    MaxResultsCount = 100,
                    ProfileConstraints =
                    new PlayerProfileViewConstraints() { ShowDisplayName = true }
                };

            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    PlayFabUserList.Add(result.Leaderboard[j]);
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }
            },
            (error) => { });
        }
    }
    void SetData(string curData)
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { "Home", curData } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => Debug.Log("������ ���� ����"));
    }

    void GetData(string curID)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
        UserRoomDataText.text = curID + "\n" + result.Data["Home"].Value,
        (error) => Debug.Log("������ �ҷ����� ����"));
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
    void Update() => LobbyInfoText.text =
        "�κ� : " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms)
        + " / ���� : " + PhotonNetwork.CountOfPlayers;
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
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            UserNickNameText.text += PhotonNetwork.PlayerList[i].NickName + "\n";
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
}