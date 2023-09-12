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
        // 포톤 최적화
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    #region 플레이팹
    // 이메일 충족 조건 : '@', '.' 이 있어야함
    // 비밀번호 충족 조건 : 6~100 자의 문자
    // 이름 충족 조건 : 3~20 자의 문자
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        //PlayFabClientAPI.LoginWithEmailAddress(request, (result) => Debug.Log("로그인 성공"), (error) => Debug.Log("로그인 실패"));
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetLeaderboard(result.PlayFabId); PhotonNetwork.ConnectUsingSettings(); },
            (error) => Debug.Log("로그인 실패"));

    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest
        { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };
        //PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Debug.Log("회원가입 성공"), (error) => Debug.Log("회원가입 실패"));
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { Debug.Log("회원가입 성공"); SetStat(); SetData("default"); },
            (error) => Debug.Log("회원가입 실패"));

    }
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => Debug.Log("값 저장실패"));
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
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => Debug.Log("데이터 저장 실패"));
    }

    void GetData(string curID)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
        UserRoomDataText.text = curID + "\n" + result.Data["Home"].Value,
        (error) => Debug.Log("데이터 불러오기 실패"));
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
        "로비 : " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms)
        + " / 접속 : " + PhotonNetwork.CountOfPlayers;
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
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            UserNickNameText.text += PhotonNetwork.PlayerList[i].NickName + "\n";
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
}