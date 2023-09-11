using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[InitializeOnLoad]
public class CtxtMenuLocalizeText : MonoBehaviour
{
    //TODO : [교수님] 프리팹가져와서 캐싱해놨다가 그녀석 가져와서 ~~~할거야
#if UNITY_EDITOR
#endif

    [MenuItem("GameObject/UI/Custom LocalizeText", false)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // 프리팹을 Resources/Prefabs 폴더에서 가져옴
        GameObject prefab = Resources.Load<GameObject>("[MJ]Prefabs/Text_Localize");

        if (prefab == null)
        {
            Debug.LogError("Text_Localize 프리팹을 찾을 수 없습니다. Resources 폴더에 올바르게 배치되었는지 확인하세요.");
            return;
        }

        // 프리팹을 복제하여 씬에 추가합니다.
        GameObject go = Instantiate(prefab);

        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

        Selection.activeObject = go;
    }
}
