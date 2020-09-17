using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlTank : BaseTank {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        MoveUpdate();
        TurretUpdate();

    }
    //炮塔控制
    public void TurretUpdate()
    {
        //
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
    //移动控制
    public void MoveUpdate()
    {
        //旋转
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        //前进后退
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
    }
}
