using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgMove : MsgBase {

	public MsgMove() { protoName = "MsgMove"; }
    public int x = 0;
    public int y = 0;
    public int z = 0;
}
public class MsgAttack : MsgBase
{
    public MsgAttack() { protoName = "MsgAttack"; }
    public string desc = "127.0.0.1:6543";

}
[System.Serializable]
public class TankInfo
{
    public int id = -1;
    public string name = "";
    public int camp = 0;
    public float hp = 0;

    public float x = 0;
    public float y = 0;
    public float z = 0;

    public float ex = 0;
    public float ey = 0;
    public float ez = 0;

}

public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle()
    {
        protoName = "MsgEnterBattle";
    }
    public TankInfo[] tanks;
    public int mapId = 1;   //地图,目前仅1张
}
public class MsgBattleResult : MsgBase
{
    public MsgBattleResult()
    {
        protoName = "MsgBattleResult";
    }
    public int winCamp = 0;
}
public class MsgLeaveBattle : MsgBase
{
    public MsgLeaveBattle()
    {
        protoName = "MsgLeaveBattle";
    }
    public int id = -1;
}
