using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlTank : BaseTank {
    private float lastSendSyncTime = 0;
    public static float syncInterval = 0.02f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        MoveUpdate();
        TurretUpdate();
        FireUpdate();

        SyncUpdate();
    }
    public void SyncUpdate()
    {
        if(Time.time - lastSendSyncTime < syncInterval)
        {
            return;
        }
        lastSendSyncTime = Time.time;

        MsgSyncTank msg = new MsgSyncTank();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        msg.turretY = turret.localEulerAngles.y;
        NetManager.Send(msg);
    }
    //炮塔控制
    public void TurretUpdate()
    {
        //
        if (IsDie())
        {
            return;
        }
        float axis = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            axis = -1;
        } else if (Input.GetKey(KeyCode.E))
        {
            axis = 1;
        }
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
        if (!Input.GetKey(KeyCode.Space))
        {
            return;
        }
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
        //旋转
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        //前进后退
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
    }
}
