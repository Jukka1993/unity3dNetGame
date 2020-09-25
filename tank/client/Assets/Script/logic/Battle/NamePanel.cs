using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamePanel : BasePanel {
    public GameObject nameTemplate;
    public Transform container;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/NamePanel";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        nameTemplate = skin.transform.Find("nameTemplate").gameObject;
        container = skin.transform.Find("Container");

    }
    public override void OnClose()
    {
        base.OnClose();
    }
    public Text GenerateName(string name)
    {
        GameObject nameObj = Instantiate(nameTemplate);
        nameObj.SetActive(true);
        nameObj.transform.SetParent(container);
        nameObj.GetComponent<Text>().text = name;
        return nameObj.GetComponent<Text>();
    }

}
