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
    private void Start()
    {
        NetManager.updateText = updateText;
        NetManager.updateText2 = updateText2;
        NetManager.updateText3 = updateText3;
        NetManager.updateText4 = updateText4;
        //网络监听
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);

        NetManager.AddMsgListener("MsgKick", OnMsgKick);

        NetManager.Connect("192.168.100.12", 8888);
        //NetManager.Connect("192.168.100.12", 8888);
        //NetManager.Connect("127.0.0.1", 8888);




        //ui管理器初始化
        PanelManager.Init();
        BattleManager.Init();
        //打开登录面板
        PanelManager.Open<LoginPanel>();
        luaEnv = new XLua.LuaEnv();
        luaEnv.DoString(luaMian.text);
        luaUpdate = luaEnv.Global.Get<UpdateDelType>("Update");
    }
    public void OnDestroy()
    {
        luaUpdate = null;
        luaEnv.Dispose();
    }
    private void Update()
    {
        text.text = showText;
        text2.text = showText2;
        text3.text = showText3;
        text4.text = showText4;
        NetManager.Update();
        if(luaUpdate != null)
        {
            luaUpdate();
        }
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
    }
    void OnMsgKick(MsgBase msgBase)
    {
        Debug.Log("被踢了");
    }
}
