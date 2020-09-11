using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour {
    //皮肤路径
    public string skinPath;
    //皮肤
    public GameObject skin;
    //层级
    public PanelManager.Layer layer = PanelManager.Layer.Panel;
    public void Init()
    {
        GameObject skinPrefab = ResManager.LoadPrefab(skinPath);
        skin = (GameObject)Instantiate(skinPrefab);
    }
    public void Close()
    {
        string name = this.GetType().ToString();
        PanelManager.Close(name);
    }
    //初始化时
    public virtual void OnInit()
    {

    }
    //显示时
    public virtual void OnShow(params object[] para)
    {

    }
    //关闭时
    public virtual void OnClose()
    {

    }

}
