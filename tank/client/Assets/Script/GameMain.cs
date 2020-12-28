//using Assets.Script.proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour {
    public static int id = -1;
    public static string userName = "";
    public TextAsset luaMian;
    XLua.LuaEnv luaEnv;
    delegate void UpdateDelType();
    UpdateDelType luaUpdate;
    public Text text;
    public Text text2;
    public Text text3;
    public Text text4;
    private string showText = "";
    private string showText2 = "";
    private string showText3 = "";
    private string showText4 = "";
    public Text text5;
    public InputField inputField;
    public InputField ipInputField;
    public bool started = false;
    private float lastCheckPingSec = 0;
    public Text c2sPing;
    public Text sDeal;
    public Text s2cPing;
    private bool shouldOpenReConnect = false;
    public void updateText(string tt)
    {
        showText = tt;
    }
    public void updateText2(string tt)
    {
        showText2 = tt;
    }
    public void updateText3(string tt)
    {
        showText3 = tt;
    }
    public void updateText4(string tt)
    {
        showText4 = tt;
    }
    public void onStartClick()
    {
        DoStart();
    }
    public void DoConnect()
    {
        NetManager.Connect(ipInputField.text, 8888);
    }
    private void DoStart()
    {
        if (started)
        {
            return;
        }
        NetManager.updateText = updateText;
        NetManager.updateText2 = updateText2;
        NetManager.updateText3 = updateText3;
        NetManager.updateText4 = updateText4;
        //网络监听
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);

        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetManager.AddMsgListener("TestMsg", OnTestMsg);

        //NetManager.Connect("192.168.100.12", 8888);
        //NetManager.Connect("172.18.10.121", 8888);
        DoConnect();
        //NetManager.Connect("127.0.0.1", 8888);




        //ui管理器初始化
        PanelManager.Init();
        BattleManager.Init();
        //打开登录面板
        PanelManager.Open<LoginPanel>();
        luaEnv = new XLua.LuaEnv();
        luaEnv.DoString(luaMian.text);
        luaUpdate = luaEnv.Global.Get<UpdateDelType>("Update");
        started = true;
    }
    public void OnTestMsg(MsgBase msg)
    {
        TestMsg msg1 = (TestMsg)msg;
        
        string s = msg1.str;
        int count = int.Parse(inputField.text);
        for (int i = 0; i < count; i++)
        {
            char[] ss = s.ToCharArray();
            for (int j = 0; j < ss.Length; j++)
            {
                s += ss[j];
            }
        }
        text5.text = msg1.msgSeq + s;
    }
    public void OnDestroy()
    {
        luaUpdate = null;
        luaEnv.Dispose();
    }
    private void Update()
    {
        if (!started)
        {
            return;
        }
        checkPing();
        text.text = showText;
        text2.text = showText2;
        text3.text = showText3;
        text4.text = showText4;
        NetManager.Update();
        if(luaUpdate != null)
        {
            luaUpdate();
        }
        tryOpenReConnect();
    }
    void tryOpenReConnect()
    {
        if (shouldOpenReConnect)
        {
            PanelManager.Open<ReConnectPanel>();
            shouldOpenReConnect = false;
        }
    }
    void checkPing()
    {
        //if (Time.time > lastCheckPingSec)
        //{
        //    PingCheck msg = new PingCheck();
        //    msg.clientSendTime1 = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        //    NetManager.Send(msg);
        //    lastCheckPingSec = Time.time;
        //}
        CheckPing.Instance.afterCheckPing = afterCheckPing;
        CheckPing.Instance.checkPing();
    }
    void afterCheckPing(double pingTime)
    {
        c2sPing.text = pingTime.ToString();
    }
    void OnConnectFail(string txt)
    {
        Debug.Log("连接失败");
    }
    void OnConnectSucc(string txt)
    {
        Debug.Log("连接成功");
    }
    void OnConnectClose(string txt)
    {
        Debug.Log("连接断开");
        shouldOpenReConnect = true;
    }
    void OnMsgKick(MsgBase msgBase)
    {
        Debug.Log("被踢了");
    }
}
