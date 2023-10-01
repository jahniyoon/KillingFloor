using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]   // 인스펙터에서 보기 위해 직렬화
public class Lang
{
    public string lang, langLocalize;
    public List<string> value = new List<string>(); //text 담을 List
}

public class Singleton : MonoBehaviour
{
    const string langURL = "https://docs.google.com/spreadsheets/d/17OfoYRzHY7psQCTczfuZwVxX5xT-GevQhSDxfmqqlXE/export?format=tsv";
    public int curLangIndex;    // 설정한 현재 언어 저장하기 위해

    public List<Lang> Langs;    // 로컬라이징할 언어별로 Class 생성
                                // 설정한 언어를 저장해야하니 초기화 X

    public event System.Action LocalizeChanged = () => { };
    public event System.Action LocalizeSettingChanged = () => { };

    #region 로컬라이징 싱글톤
    public static Singleton local_singleton;
    private void Awake()
    {
        if (local_singleton == null)
        {
            local_singleton = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
        InitLang();
    }
    #endregion

    void InitLang()
    {
        PlayerPrefs.DeleteAll();

        int langIndex = PlayerPrefs.GetInt("LangIndex", -1);    // LangIndex가 비었을 때는
                                                                // 기본값을 -1로 설정
        int systemIndex = Langs.FindIndex(x => x.lang.ToLower() == 
            Application.systemLanguage.ToString().ToLower());
        if (systemIndex == -1) systemIndex = 0;     // 설정한 언어를 찾지 못하면 영어를 기본으로
        int index = langIndex == -1 ? systemIndex : langIndex;

        SetLangIndex(index);
    }

    public void SetLangIndex(int index)
    {
        curLangIndex = index;
        PlayerPrefs.SetInt("LangIndex", curLangIndex);  // LangIndex에 설정한 언어 저장
        LocalizeChanged();
        LocalizeSettingChanged();
    }

    // 플레이 하지 않아도 언어 가져올 수 있도록
    [ContextMenu("언어 가져오기")]
    void GetLang()
    {
        StartCoroutine(GetLangCo());
    }

    IEnumerator GetLangCo()
    {
        UnityWebRequest www = UnityWebRequest.Get(langURL);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);    // 저장된 내용들 디버깅
        SetLangList(www.downloadHandler.text);  // 저장된 내용 불러와서 2차원 배열에 저장
    }

    void SetLangList(string tsv)
    {
        string[] row = tsv.Split('\n'); // 엔터 단위로 행 저장
        int rowSize = row.Length;
        int columnSize = row[0].Split('\t').Length; // 탭 단위로 열 저장
        string[,] sentence = new string[rowSize, columnSize];

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');

            for (int j = 0; j < columnSize; j++) sentence[i, j] = column[j];
        }

        // 클래스 리스트 - editor에서 미리 클래스 저장
        Langs = new List<Lang>();
        for (int i = 0; i < columnSize; i++)
        {
            Lang lang = new Lang();
            lang.lang = sentence[0, i];
            lang.langLocalize = sentence[1, i];

            for (int j = 2; j < rowSize; j++) lang.value.Add(sentence[j, i]);
            Langs.Add(lang);
        }
    }
}