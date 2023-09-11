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
    [Header("Disconnect")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();
    public InputField EmailInput, PasswordInput, UsernameInput;

    [Header("Lobby")]
    public InputField UserNickNameInput;
    public Text LobbyInfoText, UserNickNameText;

    [Header("Room")]
    public InputField SetDataInput;
    public GameObject SetDataBtnObj;
    public Text UserHouseDataText, RoomNameInfoText, RoomNumInfoText;

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
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        //PlayFabClientAPI.LoginWithEmailAddress(request, (result) => Debug.Log("�α��� ����"), (error) => Debug.Log("�α��� ����"));
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetLeaderboard(result.PlayFabId); PhotonNetwork.ConnectUsingSettings(); },
            (error) => print("�α��� ����"));

    }
    
    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text };
        //PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Debug.Log("ȸ������ ����"), (error) => Debug.Log("ȸ������ ����"));
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { print("ȸ������ ����"); SetStat(); SetData("default"); },
            (error) => print("ȸ������ ����"));

    }
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("�� �������"));
    }
    void GetLeaderboard(string myID)
    {
        PlayFabUserList.Clear();

        for (int i = 0; i < 10; i++)
        {
            var request = new GetLeaderboardRequest
            {
                StartPosition = i * 100,
                StatisticName = "IDInfo",
                MaxResultsCount = 100,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
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
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => print("������ ���� ����"));
    }

    void GetData(string curID)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
        UserHouseDataText.text = curID + "\n" + result.Data["Home"].Value,
        (error) => print("������ �ҷ����� ����"));
    }
    #endregion

    #region LobbyScene
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    //void Update() => LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�κ� / " + PhotonNetwork.CountOfPlayers + "����";
    public override void OnJoinedLobby()
    {
        // �濡�� �κ�� �� �� �����̾���, �α����ؼ� �κ�� �� �� PlayFabUserList�� ä���� �ð����� 1�� ������
        if (isLoaded)
        {
            ShowUserNickName();
        }
        else Invoke("OnJoinedLobbyDelay", 3);
    }
    void OnJoinedLobbyDelay()
    {
        isLoaded = true;
        PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;

        ShowUserNickName();
    }
    void ShowUserNickName()
    {
        UserNickNameText.text = "";
        for (int i = 0; i < PlayFabUserList.Count; i++) UserNickNameText.text += PlayFabUserList[i].DisplayName + "\n";
    }

    public void XBtn()
    {
        if (PhotonNetwork.InLobby) PhotonNetwork.Disconnect();
        else if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isLoaded = false;
    }

    #endregion

    //void OnLoginSuccess(LoginResult result) => Debug.Log("�α��� ����");

    //void OnLoginFailure(PlayFabError error) => Debug.Log("�α��� ����");

    //void OnRegisterSuccess(RegisterPlayFabUserResult result) => Debug.Log("ȸ������ ����");

    //void OnRegisterFailure(PlayFabError error) => Debug.Log("ȸ������ ����");
}