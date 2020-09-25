using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUIPanel : BasePanel {
    public GameObject tankUITemplate;
    public Transform container;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/TankUIPanel";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        tankUITemplate = skin.transform.Find("tankUI").gameObject;
        container = skin.transform.Find("Container");

    }
    public override void OnClose()
    {
        base.OnClose();
    }
    public TankUI GenerateTankUI(string name,float hp)
    {
        GameObject nameObj = Instantiate(tankUITemplate);
        nameObj.SetActive(true);
        nameObj.transform.SetParent(container);
        TankUI tankUI = nameObj.AddComponent<TankUI>();
        tankUI.Init(name, hp);
        return tankUI;
    }

}
