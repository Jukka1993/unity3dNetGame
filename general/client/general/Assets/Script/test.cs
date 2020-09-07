using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class test : MonoBehaviour {
    public InputField nameInputField;
    public InputField passwordInputField;
    public InputField notePadInputField;

	// Use this for initialization
	void Start () {
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
	}
    void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if(msg.result == 0)
        {
            Debug.Log("注册成功");
        } else
        {
            Debug.Log("注册失败");
        }

    }
    void OnMsgMove(MsgBase msgBase)
    {
        MsgMove msg = (MsgMove)msgBase;
        //Debug.Log("OnMsgMove msg.x = " + msg.x);
        //Debug.Log("OnMsgMove msg.y = " + msg.y);
        //Debug.Log("OnMsgMove msg.z = " + msg.z);
        Debug.Log("Receive OnMsgMove => " + msg.x + " " + msg.y + " " + msg.z);


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
        Debug.Log("AA");
        NetManager.Connect("127.0.0.1", 8888);
        //todo 转圈圈,开始连接中
    }
    public void OnRegisterClicked()
    {
        Debug.Log("OnRegisterClicked ");
        Debug.Log(nameInputField.text);
        Debug.Log(passwordInputField.text);

        MsgRegister msgRegister = new MsgRegister();
        msgRegister.name = nameInputField.text;
        msgRegister.pw = passwordInputField.text;

        NetManager.Send(msgRegister);
    }

    public void OnCloseClicked()
    {
        NetManager.Close();
    }
    public void OnMoveClicked()
    {
        Debug.Log("OnMoveClicked 111");
        MsgMove msg = new MsgMove();
        msg.x = 120;
        msg.y = 123;
        msg.z = -6;
        Debug.Log("Send OnMsgMove => " + msg.x+" "+msg.y + " " + msg.z);
        NetManager.Send(msg);
    }

    // Update is called once per frame
    void Update () {
        NetManager.Update();
	}
}
