using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel {
    private InputField nameInputField;
    private InputField passwordInputField;
    private InputField passwordInputField1;
    private Button registerButton;
    private Button closeButton;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/RegisterView";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        //寻找组件
        nameInputField = skin.transform.Find("NameInputField").GetComponent<InputField>();
        passwordInputField = skin.transform.Find("PasswordInputField").GetComponent<InputField>();
        passwordInputField1 = skin.transform.Find("PasswordInputField1").GetComponent<InputField>();
        registerButton = skin.transform.Find("RegisterButton").GetComponent<Button>();
        closeButton = skin.transform.Find("CloseButton").GetComponent<Button>();

        //事件监听
        registerButton.onClick.AddListener(OnRegisterClicked);
        closeButton.onClick.AddListener(OnCloseClicked);

        //网络消息监听
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    }
    public void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if(msg.result == 0)
        {
            CommonUtil.OpenTip("注册成功");
            Close();
        }
        else
        {
            CommonUtil.OpenTip("注册失败 " + msg.reasonStr);
        }
    }
    public void OnRegisterClicked()
    {
        Debug.Log("========== OnRegisterClicked== " + nameInputField.text + " " + passwordInputField.text + " " + passwordInputField1.text);
        if (nameInputField.text == "")
        {
            CommonUtil.OpenTip("姓名不可为空");
            return;
        }
        if(passwordInputField.text == "")
        {
            CommonUtil.OpenTip("密码不可为空");
            return;
        }
        if(passwordInputField1.text == "")
        {
            CommonUtil.OpenTip("请重复输入密码");
            return;
        }
        if(passwordInputField.text != passwordInputField1.text)
        {
            CommonUtil.OpenTip("两次密码输入不同");
            return;
        }
        MsgRegister msg = new MsgRegister();
        msg.name = nameInputField.text;
        msg.pw = passwordInputField.text;
        NetManager.Send(msg);
    }
    public void OnCloseClicked()
    {
        Close();
    }
}
