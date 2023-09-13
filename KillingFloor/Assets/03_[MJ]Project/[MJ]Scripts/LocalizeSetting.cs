using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Singleton;

public class LocalizeSetting : MonoBehaviour
{
    Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        if (dropdown.options.Count != local_singleton.Langs.Count) SetLangOption();
        dropdown.onValueChanged.AddListener((d) => local_singleton.SetLangIndex(dropdown.value));

        LocalizeSettingChanged();
        local_singleton.LocalizeSettingChanged += LocalizeSettingChanged;
    }

    void OnDestroy()
    {
        local_singleton.LocalizeSettingChanged -= LocalizeSettingChanged;
    }

    void SetLangOption()
    {
        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        for (int i = 0; i < local_singleton.Langs.Count; i++)
            optionDatas.Add(new Dropdown.OptionData() { text = local_singleton.Langs[i].langLocalize });
        dropdown.options = optionDatas;
    }

    void LocalizeSettingChanged()
    {
        dropdown.value = local_singleton.curLangIndex;
    }
}
