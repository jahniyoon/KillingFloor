using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Singleton;

public class LocalizeScript : MonoBehaviour
{
    public string textKey;

    void Start()
    {
        LocalizeChanged();
        local_singleton.LocalizeChanged += LocalizeChanged;
    }

    void OnDestroy()
    {
        local_singleton.LocalizeChanged -= LocalizeChanged;
    }

    string Localize(string key)
    {
        int keyIndex = local_singleton.Langs[0].value.FindIndex(x => x.ToLower() == key.ToLower());
        return local_singleton.Langs[local_singleton.curLangIndex].value[keyIndex];
    }

    void LocalizeChanged()
    {
        if (GetComponent<Text>() != null)
            GetComponent<Text>().text = Localize(textKey);
    }
}