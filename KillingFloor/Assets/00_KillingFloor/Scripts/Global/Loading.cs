using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public void Run(float t, string name)
    {
        StartCoroutine(DelayForLoadScene(t, name));
    }

    public void RunForPause(float t, string name)
    {
        StartCoroutine(DelayForLoadSceneForPause(t, name));
    }

    // 비동기 로딩 씬(미리 로딩 완료하고 원하는 타이밍에 씬 호출)
    public void asyncLoadScene(string name, float delay)
    {
        // 비동기 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false; // 씬 로딩 완료 후 자동 활성화 막기

        // 로딩이 완료될 때까지 대기
        StartCoroutine(WaitForSceneLoad(asyncLoad, delay));
    }

    public IEnumerator DelayForLoadScene(float t, string name)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(name);
    }

    public IEnumerator DelayForLoadSceneForPause(float t, string name)
    {
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < t)
        {
            yield return null;

            // 일시정지된 상태에서 시간을 계산하기 위해 unscaledDeltaTime을 사용
            if (Time.timeScale == 0.0f)
            {
                startTime += Time.unscaledDeltaTime;
            }
        }

        SceneManager.LoadScene(name);
    }

    private IEnumerator WaitForSceneLoad(AsyncOperation asyncLoad, float delay)
    {
        while (!asyncLoad.isDone)
        {
            // 로딩 진행 상황을 표시하거나 추가 작업 수행 가능
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9f는 로딩이 90%까지 완료되었다는 의미
            Debug.Log("Loading progress: " + progress * 100 + "%");

            // 조건을 만족하면 씬 활성화
            if (progress >= 1.0f)
            {
                yield return new WaitForSeconds(delay);
                asyncLoad.allowSceneActivation = true; // 씬 활성화 허용
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        // 로딩 완료 후 추가 작업 수행 가능
        Debug.Log("Scene loading complete!");
    }
}
