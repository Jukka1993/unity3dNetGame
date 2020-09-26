using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager : MonoBehaviour {
    //预设
    public static GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }
    public static TextAsset LoadText(string path)
    {
        return Resources.Load<TextAsset>(path);
    }
    public static TextAsset LoadLua(string path)
    {
        return Resources.Load<TextAsset>(path);
    }
}
