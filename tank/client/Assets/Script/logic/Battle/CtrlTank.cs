using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CtrlTank : BaseTank {
    private double lastSendSyncTime = 0;
    //public static float syncInterval = 0.02f;
    public static double syncInterval = 50f;
    //private int nameUp = 5;
    public VariableJoystick joystick;
    public EButton fireButton;
    public EButton leftButton;
    public EButton rightButton;

    public Toggle toggleLeft;
    public Toggle toggleRight;
    public Toggle toggleForward;
    public Toggle toggleBack;
    public Toggle toggleFire;
    public Toggle toggleRotateLeft;
    public Toggle toggleRotateRight;

    // Use this for initialization
    void Start () {
        //nameUp = 5;
        PlayControlPanel controlPanel = PanelManager.GetPanel<PlayControlPanel>();
        if(controlPanel != null)
        {
            joystick = controlPanel.joystick;
            leftButton = controlPanel.leftButton;
            rightButton = controlPanel.rightButton;
            fireButton = controlPanel.fireButton;

            toggleLeft = controlPanel.toggleLeft;
            toggleRight = controlPanel.toggleRight;
            toggleForward = controlPanel.toggleForward;
            toggleBack = controlPanel.toggleBack;
            toggleFire = controlPanel.toggleFire;
            toggleRotateLeft = controlPanel.toggleRotateLeft;
            toggleRotateRight = controlPanel.toggleRotateRight;
        }
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        MoveUpdate();
        TurretUpdate();
        FireUpdate();

        SyncUpdate();
        UIUpdate();
    }
    public void SyncUpdate()
    {
        double tttt = CommonUtil.GetTimeStamp() - lastSendSyncTime;
        if (tttt < syncInterval)
        {
            //Debug.Log("JJJ");
            //Debug.Log(CommonUtil.GetTimeStamp());
            //Debug.Log(lastSendSyncTime);
            return;
        }
        lastSendSyncTime = CommonUtil.GetTimeStamp();

        MsgSyncTank msg = new MsgSyncTank();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        Debug.Log(msg.ex + " " + msg.ey + " " + msg.ez);
        msg.turretY = turret.localEulerAngles.y;
        NetManager.Send(msg);
    }
    //炮塔控制
    public void TurretUpdate()
    {
        //if (Input.GetButtonDown("LeftButton"))
        //{
        //    Debug.Log("LeftButton Down");
        //}
        //
        if (IsDie())
        {
            return;
        }
        float axis = 0;
        if (leftButton.isPressed || toggleRotateLeft.isOn)
        {
            axis = -1;
        } else if (rightButton.isPressed || toggleRotateRight.isOn)
        {
            axis = 1;
        }
        //if (Input.GetKey(KeyCode.Q))
        //{
        //    axis = -1;
        //} else if (Input.GetKey(KeyCode.E))
        //{
        //    axis = 1;
        //}
        Vector3 le = turret.localEulerAngles;
        le.y += axis * Time.deltaTime * turretSpeed;
        turret.localEulerAngles = le;
    }
    public void FireUpdate()
    {
        if (IsDie())
        {
            return;
        }
        if (!fireButton.isPressed && !toggleFire.isOn)
        {
            return;
        }
        //if (!Input.GetKey(KeyCode.Space))
        //{
        //    return;
        //}
        if(Time.time - lastFireTime < fireCd)
        {
            return;
        }
        Bullet bullet = Fire();
        if(bullet != null)
        {
            MsgFire msg = new MsgFire();
            msg.x = bullet.transform.position.x;
            msg.y = bullet.transform.position.y;
            msg.z = bullet.transform.position.z;
            msg.ex = bullet.transform.eulerAngles.x;
            msg.ey = bullet.transform.eulerAngles.y;
            msg.ez = bullet.transform.eulerAngles.z;
            NetManager.Send(msg);
        }
    }
    //移动控制
    public void MoveUpdate()
    {
        if (IsDie())
        {
            return;
        }

        float x = joystick.Horizontal;
        float y = joystick.Vertical;
        if (toggleForward.isOn)
        {
            y = 1;
        } else if (toggleBack.isOn)
        {
            y = -1;
        }
        if (toggleLeft.isOn)
        {
            x = -1;
        } else if (toggleRight.isOn)
        {
            x = 1;
        }
        //float x = Input.GetAxis("Horizontal");
        //float y = Input.GetAxis("Vertical");

        //旋转
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        //前进后退
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
    }
}
