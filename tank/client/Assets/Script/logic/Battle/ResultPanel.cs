using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : BasePanel {
    public Transform winTrans;
    public Transform lostTrans;
    public Button confirmBtn;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/ResultPanel";
        layer = PanelManager.Layer.Tip;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        winTrans = skin.transform.Find("WinText");
        lostTrans = skin.transform.Find("LostText");
        confirmBtn = skin.transform.Find("Button").GetComponent<Button>();

        bool isWin = (bool)para[0];
        winTrans.gameObject.SetActive(isWin);
        lostTrans.gameObject.SetActive(!isWin);

        confirmBtn.onClick.AddListener(OnConfirmBtnClicked);
    }
    public void OnConfirmBtnClicked()
    {
        PanelManager.Open<RoomPanel>();
        Close();
    }
    public override void OnClose()
    {
        base.OnClose();
    }
}
