﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel {
    private InputField nameInputField;
    private InputField passwordInputField;
    private Button loginButton;
    private Button registerButton;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/LoginView";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        //寻找组件及组件监听
        nameInputField = skin.transform.Find("NameInputField").GetComponent<InputField>();
        passwordInputField = skin.transform.Find("PasswordInputField").GetComponent<InputField>();
        loginButton = skin.transform.Find("LoginButton").GetComponent<Button>();
        registerButton = skin.transform.Find("RegisterButton").GetComponent<Button>();
        loginButton.onClick.AddListener(OnLoginClick);
        registerButton.onClick.AddListener(OnRegisterClick);
        //网络消息监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //网络事件监听
        //NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        //NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);

        //连接服务器
        //NetManager.Connect("127.0.0.1", 8888);
    }
    public override void OnClose()
    {
        base.OnClose();
        //网络协议监听 移除
        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        //网络事件监听 移除
        //NetManager.RemoveEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        //NetManager.RemoveEventListener(NetEvent.ConnectFail, OnConnectFail);
    }
    private void OnConnectSucc(string str)
    {
        Debug.Log("============OnConnectSucc");
    }
    private void OnConnectFail(string str)
    {
        Debug.Log("============OnConnectFail");

    }
    private void OnMsgLogin(MsgBase msgBase)
    {
        Debug.Log("=======OnMsgLogin");
        MsgLogin msg = (MsgLogin)msgBase;
        if(msg.result == 0)
        {
            CommonUtil.OpenTip("登录成功");
            ////进入游戏
            ////添加坦克
            //GameObject tankObj = new GameObject("myTank");
            //CtrlTank ctrlTank = tankObj.AddComponent<CtrlTank>();
            //ctrlTank.Init("Prefabs/ModelPre/TankPrefab/tankPrefab");
            ////设置相机
            //tankObj.AddComponent<CameraFollow>();
            GameMain.id = msg.id;
            GameMain.userName = msg.name;
            PanelManager.Open<RoomListPanel>();
            //关闭登录界面
            Close();
        } else
        {
            CommonUtil.OpenTip("登录失败: " + msg.reasonStr);
        }
    }
    private void OnLoginClick()
    {
        Debug.Log("===OnLoginClick");
        //if(nameInputField.text == "" || passwordInputField.text == "")
        //{
        //    CommonUtil.OpenTip("用户名和密码不能为空");
        //    return;
        //}
        //发送登录请求
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.name = nameInputField.text;
        msgLogin.pw = passwordInputField.text;
        NetManager.Send(msgLogin);
    }
    private void OnRegisterClick()
    {
        PanelManager.Open<RegisterPanel>();
    }
}
