using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
    public static int id = -1;
    public static string userName = "";
    public TextAsset luaMian;
    XLua.LuaEnv luaEnv;
    delegate void UpdateDelType();
    UpdateDelType luaUpdate;
    private void Start()
    {
        //网络监听
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);

        NetManager.AddMsgListener("MsgKick", OnMsgKick);

        //NetManager.Connect("172.18.10.121", 8888);
        NetManager.Connect("192.168.100.12", 8888);
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
