using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ReConnectPanel : BasePanel
{
    public Button reconnectBtn;
    public Text reconnectingTip;
    public Image reconnectingImg;
    public Text disconnectTip;
    private bool isConnecting;
    private int dotCount = 0;
    private bool afterReconnectSucc;
    private bool afterReconnectFail;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/ReConnectPanel";
        layer = PanelManager.Layer.Panel;
        Debug.Log("AA0");

    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        reconnectBtn = skin.transform.Find("ReConnectButton").GetComponent<Button>();
        reconnectingTip = skin.transform.Find("ReConnectingTip").GetComponent<Text>();
        disconnectTip = skin.transform.Find("DisConnectTip").GetComponent<Text>();
        reconnectingImg = skin.transform.Find("ReConnectCircle").GetComponent<Image>();
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        setState(false);
        reconnectBtn.onClick.AddListener(onReConnectClick);
    }
    public override void OnClose()
    {
        base.OnClose();
        NetManager.RemoveEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetEvent.ConnectFail, OnConnectFail);
        
    }
    public void OnConnectSucc(string txt)
    {
        afterReconnectSucc = true;
    }
    public void tryClose()
    {
        if (afterReconnectSucc)
        {
            afterReconnectSucc = false;
            PanelManager.Close("ReConnectPanel");
        }
    }
    public void TryAfterFail()
    {
        if (afterReconnectFail)
        {
            afterReconnectFail = false;
            setState(false);
        }
    }
    public void OnConnectFail(string txt)
    {
        afterReconnectFail = true;
    }
    public void OnConnectClose(string txt)
    {

    }
    public void onReConnectClick()
    {
        setState(true);
        GameObject.Find("Root").GetComponent<GameMain>().DoConnect();
    }
    private void setState(bool isConnecting)
    {
        this.isConnecting = isConnecting;
        UpdatePanelShow();
    }
    private void UpdatePanelShow()
    {
        reconnectingImg.gameObject.SetActive(isConnecting);
        reconnectingTip.gameObject.SetActive(isConnecting);
        disconnectTip.gameObject.SetActive(!isConnecting);
        reconnectBtn.gameObject.SetActive(!isConnecting);
    }
    private void Update()
    {
        if (isConnecting && reconnectingImg.gameObject.activeInHierarchy)
        {
            reconnectingImg.transform.Rotate(new Vector3(0, 0, 1), 1);
            dotCount++;
            if (dotCount >= 400)
            {
                dotCount = 0;
            }
            int realDotCount = dotCount % 100;
            if (realDotCount == 0)
            {
                reconnectingTip.text = "重连中";
            } else if (realDotCount == 1)
            {
                reconnectingTip.text = "重连中.";
            } else if (realDotCount == 2)
            {
                reconnectingTip.text = "重连中..";
            } else if (realDotCount == 3)
            {
                reconnectingTip.text = "重连中...";
            }
        }
        tryClose();
        TryAfterFail();
    }



}
