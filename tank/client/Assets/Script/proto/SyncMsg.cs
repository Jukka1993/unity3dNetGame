using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgSyncTank : MsgBase {
    public MsgSyncTank()
    {
        protoName = "MsgSyncTank";
    }
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;
    public float turretY = 0f;  //炮塔角度
    //服务器补充
    public int id = -1; //坦克player id
}
public class MsgFire : MsgBase
{
    public MsgFire()
    {
        protoName = "MsgFire";
    }
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;

    public int id = -1;
}
public class MsgHit : MsgBase
{
    public MsgHit()
    {
        protoName = "MsgHit";
    }
    public int targetId = -1;   //被击中者id
    public float x = 0f;        //击中点
    public float y = 0f;
    public float z = 0f;
    public int id = -1;     //炮弹发射者id
    public int hp = 0;      //被击中坦克血量
    public int damage = 0;  //受到的伤害
}
