using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTank : BaseTank {
    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 forecastPos;
    private Vector3 forecastRot;
    private double forecastTime;
	
    new void Update()
    {
        base.Update();
        ForecastUpdate();
        UIUpdate();
    }
    public override void Init(string skinPath)
    {
        base.Init(skinPath);
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        rigidBody.useGravity = false;
        lastPos = transform.position;
        lastRot = transform.eulerAngles;
        forecastPos = transform.position;
        forecastRot = transform.eulerAngles;
        forecastTime = CommonUtil.GetTimeStamp();
    }
    public bool ForecastUpdate()
    {
        float t = (float)((CommonUtil.GetTimeStamp() - forecastTime) / CtrlTank.syncInterval);
        t = Mathf.Clamp(t, 0f, 1f);
        //旋转
        Quaternion quat = transform.rotation;
        Quaternion forcastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forcastQuat, t);
        transform.rotation = quat;
        //位置
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos, t);
        if (!transform.position.Equals(pos))
        {
            //Debug.Log("其他坦克理论上会动");
            transform.position = pos;
            return true;
        } else
        {
            //Debug.Log("运行到这里了导致tank不动的");
            return false;
        }
    }
    public void SyncPos(MsgSyncTank msg)
    {
        //预测位置
        Vector3 pos = new Vector3(msg.x,msg.y,msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        forecastPos = pos + 2 * (pos - lastPos);
        forecastRot = rot + 2 * (rot - lastRot);
        //更新
        lastPos = pos;
        lastRot = rot;
        forecastTime = CommonUtil.GetTimeStamp();
        Vector3 le = turret.localEulerAngles;
        le.y = msg.turretY;
        turret.localEulerAngles = le;
    }
    public void SyncFire(MsgFire msg)
    {
        Bullet bullet = Fire();
        if(bullet != null)
        {
            Vector3 pos = new Vector3(msg.x,msg.y,msg.z);
            Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
            bullet.transform.position = pos;
            bullet.transform.eulerAngles = rot;
        }
    }
}
