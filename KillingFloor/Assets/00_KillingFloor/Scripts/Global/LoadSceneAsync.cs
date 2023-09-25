using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviourPun
{
    private bool loadComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitPlayersLoadingComplete());
    }

    public void AsyncLoadScene(string name)
    {
        // 비동기 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false; // 씬 로딩 완료 후 자동 활성화 막기

        // 로딩이 완료될 때까지 대기
        StartCoroutine(WaitForSceneLoad(asyncLoad));
    }

    private IEnumerator WaitForSceneLoad(AsyncOperation asyncLoad)
    {
        while (!asyncLoad.isDone)
        {
            // 로딩 진행 상황을 표시하거나 추가 작업 수행 가능
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.99f는 로딩이 90%까지 완료되었다는 의미
            Debug.Log("Loading progress: " + progress * 100 + "%");

            // 조건을 만족하면 씬 활성화
            if (progress >= 1.0f)
            {
                if (CheckIfAllPlayersNextLoad())
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        yield return new WaitForSeconds(1.0f);
                    }

                        asyncLoad.allowSceneActivation = true; // 씬 활성화 허용
                }
            }

            // 다음 프레임까지 대기
            yield return null;
        }
    }

    private bool CheckIfAllPlayersNextLoad()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object loadingProgressObj;
            if (player.CustomProperties.TryGetValue("LoadingProgress", out loadingProgressObj))
            {
                float loadingProgress = (float)loadingProgressObj;
                if (loadingProgress < 1.0f)
                {
                    // 로딩 진행 상황이 1.0 미만인 플레이어가 있으면 false 반환
                    return false;
                }
            }
        }

        // 모든 플레이어의 로딩 진행 상황이 1.0 이상이면 true 반환
        return true;
    }

    private IEnumerator WaitPlayersLoadingComplete()
    {
        while (!loadComplete)
        {
            // 모든 플레이어가 로딩을 완료했는지 확인
            bool allPlayersLoaded = CheckIfAllPlayersLoaded();

            if (allPlayersLoaded)
            {
                // 모든 플레이어가 로딩을 완료하면 씬 로딩 시작
                AsyncLoadScene("Main");
                loadComplete = true;
            }

            yield return null; // 일정 간격으로 확인
        }
    }

    private bool CheckIfAllPlayersLoaded()
    {
        // 모든 플레이어의 로딩 상태를 확인
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object loadingCompleteObj;
            if (player.CustomProperties.TryGetValue("LoadingComplete", out loadingCompleteObj))
            {
                bool loadingComplete = (bool)loadingCompleteObj;
                if (!loadingComplete)
                {
                    // 로딩이 완료되지 않은 플레이어가 있으면 false 반환
                    return false;
                }
            }
        }

        // 모든 플레이어가 로딩을 완료했으면 true 반환
        return true;
    }
}