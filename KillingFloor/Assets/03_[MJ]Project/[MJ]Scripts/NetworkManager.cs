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
        // 포톤 최적화
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    #region 플레이팹
    // 이메일 충족 조건 : '@', '.' 이 있어야함
    // 비밀번호 충족 조건 : 6~100 자의 문자
    // 이름 충족 조건 : 3~20 자의 문자
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        //PlayFabClientAPI.LoginWithEmailAddress(request, (result) => Debug.Log("로그인 성공"), (error) => Debug.Log("로그인 실패"));
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetLeaderboard(result.PlayFabId); PhotonNetwork.ConnectUsingSettings(); },
            (error) => print("로그인 실패"));

    }
    
    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text };
        //PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Debug.Log("회원가입 성공"), (error) => Debug.Log("회원가입 실패"));
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { print("회원가입 성공"); SetStat(); SetData("default"); },
            (error) => print("회원가입 실패"));

    }
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("값 저장실패"));
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
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => print("데이터 저장 실패"));
    }

    void GetData(string curID)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
        UserHouseDataText.text = curID + "\n" + result.Data["Home"].Value,
        (error) => print("데이터 불러오기 실패"));
    }
    #endregion

    #region LobbyScene
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    //void Update() => LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    public override void OnJoinedLobby()
    {
        // 방에서 로비로 올 땐 딜레이없고, 로그인해서 로비로 올 땐 PlayFabUserList가 채워질 시간동안 1초 딜레이
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

    //void OnLoginSuccess(LoginResult result) => Debug.Log("로그인 성공");

    //void OnLoginFailure(PlayFabError error) => Debug.Log("로그인 실패");

    //void OnRegisterSuccess(RegisterPlayFabUserResult result) => Debug.Log("회원가입 성공");

    //void OnRegisterFailure(PlayFabError error) => Debug.Log("회원가입 실패");
}