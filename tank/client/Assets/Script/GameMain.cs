using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
    public static int id = -1;
    public static string userName = "";
    private void Start()
    {
        //网络监听
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);

        NetManager.AddMsgListener("MsgKick", OnMsgKick);

        NetManager.Connect("127.0.0.1", 8888);


        //ui管理器初始化
        PanelManager.Init();
        //打开登录面板
        PanelManager.Open<LoginPanel>();
    }
    private void Update()
    {
        NetManager.Update();
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
