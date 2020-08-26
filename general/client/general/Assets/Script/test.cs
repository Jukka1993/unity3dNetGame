using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
	}
    void OnMsgMove(MsgBase msgBase)
    {
        MsgMove msg = (MsgMove)msgBase;
        Debug.Log("OnMsgMove msg.x = " + msg.x);
        Debug.Log("OnMsgMove msg.y = " + msg.y);
        Debug.Log("OnMsgMove msg.z = " + msg.z);

    }
    void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc => "+err);
        //todo 进入游戏
    }
    void OnConnectFail(string err)
    {
        Debug.Log("OnConnectFail => " + err);
        //todo 进入游戏
    }
    void OnConnectClose(string err)
    {
        Debug.Log("OnConnectClose => "+err);
        //todo 进入游戏
    }
    public void OnConnectClicked()
    {
        NetManager.Connect("127.0.0.1", 8888);
        //todo 转圈圈,开始连接中
    }
    public void OnCloseClicked()
    {
        NetManager.Close();
    }
    public void OnMoveClicked()
    {
        Debug.Log("OnMoveClicked");
        MsgMove msg = new MsgMove();
        msg.x = 120;
        msg.y = 123;
        msg.z = -6;
        NetManager.Send(msg);
    }

    // Update is called once per frame
    void Update () {
        NetManager.Update();
	}
}
