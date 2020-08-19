using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);
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

    // Update is called once per frame
    void Update () {
		
	}
}
