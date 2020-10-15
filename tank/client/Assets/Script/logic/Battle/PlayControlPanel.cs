using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayControlPanel : BasePanel
{
    public VariableJoystick joystick;
    public EButton leftButton;
    public EButton rightButton;
    public EButton fireButton;
    public bool leftButtonPressed = false;
    public bool rightButtonPressed = false;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/PlayControlPanel";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnClose()
    {
        base.OnClose();
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        joystick = skin.transform.Find("Variable Joystick").gameObject.GetComponent<VariableJoystick>();;
        rightButton = skin.transform.Find("RightButton").GetComponent<EButton>();
        leftButton = skin.transform.Find("LeftButton").GetComponent<EButton>();
        fireButton = skin.transform.Find("FireButton").GetComponent<EButton>();
    }
}
