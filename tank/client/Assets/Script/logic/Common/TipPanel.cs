using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel {

    public Button confirmButton;
    public Text content;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/TipPanel";
        layer = PanelManager.Layer.Tip;
        
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        //寻找组件
        confirmButton = skin.transform.Find("ConfirmButton").GetComponent<Button>();
        content = skin.transform.Find("Content").GetComponent<Text>();
        //组件监听
        confirmButton.onClick.AddListener(OnCloseClick);
        //提示语
        if(para.Length == 1)
        {
            content.text = (string)para[0];
        }
    }
    public void OnCloseClick()
    {
        Close();
    }
}
