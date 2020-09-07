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
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetManager.AddMsgListener("MsgLogout", OnMsgLogout);
        NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);
        NetManager.AddMsgListener("MsgGetText", OnGetMsgGetText);
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
    void OnMsgLogout(MsgBase msgBase)
    {
        MsgLogout msg = (MsgLogout)msgBase;
        if(msg.result == 0)
        {
            Debug.Log("登出成功");
        } else
        {
            Debug.Log("登出失败");
        }
    }
    void OnMsgKick(MsgBase msgBase)
    {
        MsgKick msg = (MsgKick)msgBase;
        Debug.Log("被踢了");
    }
    void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if(msg.result == 0)
        {
            Debug.Log("登录成功");
            //this.gameObject.active = false;
            GetMsgGetText();
        } else
        {
            Debug.Log("登录失败");
        }
    }
    void OnGetMsgGetText(MsgBase msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        notePadInputField.text = msg.text;
    }
    void OnMsgSaveText(MsgBase msgBase)
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        if(msg.result == 0)
        {
            Debug.Log("保存成功");
        } else
        {
            Debug.Log("保存失败");
        }
    }
    public void SendMsgSaveText()
    {
        Debug.Log("SendMsgSaveText");
        MsgSaveText msg = new MsgSaveText();
        msg.text = notePadInputField.text;
        NetManager.Send(msg);
    }
    public void GetMsgGetText()
    {
        MsgGetText msg = new MsgGetText();
        NetManager.Send(msg);
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
    public void OnLoginClicked()
    {
        Debug.Log("OnLoginClicked");
        MsgLogin msg = new MsgLogin();
        msg.name = nameInputField.text;
        msg.pw = passwordInputField.text;
        NetManager.Send(msg);
    }
    public void OnLogoutClicked()
    {
        Debug.Log("OnLogoutClicked");
        MsgLogout msg = new MsgLogout();
        NetManager.Send(msg);
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
