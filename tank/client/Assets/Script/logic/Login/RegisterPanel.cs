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
        skinPath = "";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        //寻找组件

    }

}
