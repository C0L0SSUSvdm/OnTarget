using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Utility_UseCommonPrefab : MonoBehaviour
{
    [SerializeField] GameObject Prefab;

#if UNITY_EDITOR
    [ContextMenu("Use Common Prefab")]

    public void ConvertChildrenToPrefab()
    {
        GameObject[] list = new GameObject[transform.childCount];
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            list[i] = transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < list.Length; i++)
        {
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(Prefab, gameObject.transform);
            newObject.transform.SetAsLastSibling();

            //children[i] = new Transform(list[i].transform);

            DestroyImmediate(list[i]);
            
            
            PrefabUtility.RevertPrefabInstance(newObject, InteractionMode.AutomatedAction);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).transform.position = children[i].transform.position;
            gameObject.transform.GetChild(i).transform.rotation = children[i].transform.rotation;
            gameObject.transform.GetChild(i).transform.localScale = children[i].transform.localScale;
            gameObject.transform.GetChild(i).transform.name = children[i].transform.name;
        }

    }
#endif
}
