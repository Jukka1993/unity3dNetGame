using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    public static Dictionary<int, BaseTank> tanks = new Dictionary<int, BaseTank>();

	public static void Init()
    {
        RemoveMsgListener();
        AddMsgListener();
    }
    private static void RemoveMsgListener()
    {
        NetManager.RemoveMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        NetManager.RemoveMsgListener("MsgBattleResult", OnMsgBattleResult);
        NetManager.RemoveMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
        NetManager.RemoveMsgListener("MsgSyncTank", OnMsgSyncTank);
        NetManager.RemoveMsgListener("MsgFire", OnMsgFire);
        NetManager.RemoveMsgListener("MsgHit", OnMsgHit);
    }
    private static void AddMsgListener()
    {
        NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
        NetManager.AddMsgListener("MsgSyncTank", OnMsgSyncTank);
        NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);
    }
    private static void OnMsgSyncTank(MsgBase msgBase)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBase;
        if(msg.id == GameMain.id)
        {
            Debug.LogError(msg.id + " " + GameMain.id);
            return;
        }
        Debug.Log("收到其他人的坦克同步信息");
        SyncTank tank = (SyncTank)GetTank(msg.id);
        if(tank == null)
        {
            Debug.Log("收到其他人的坦克同步信息,但是tank==null");
            return;
        }
        tank.SyncPos(msg);
    }
    private static void OnMsgFire(MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        if(msg.id == GameMain.id)
        {
            return;
        }
        SyncTank tank = (SyncTank)GetTank(msg.id);
        if(tank == null)
        {
            return;
        }
        tank.SyncFire(msg);
    }
    private static void OnMsgHit(MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        BaseTank tank = GetTank(msg.targetId);
        if(tank == null)
        {
            return;
        }
        tank.Attacked(msg.damage);
    }
    private static void OnMsgEnterBattle(MsgBase msgBase)
    {
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;
        EnterBattle(msg);
    }
    public static void ReEnterBattle(MsgReEnterRoom msg)
    {
        //EnterBattle((MsgEnterBattle)msg);
        Reset();
        PanelManager.Close("RoomPanel");
        PanelManager.Close("ResultPanel");
        PanelManager.Open<TankUIPanel>();
        PanelManager.Open<PlayControlPanel>();
        for (int i = 0; i < msg.tanks.Length; i++)
        {
            GenerateTank(msg.tanks[i]);
        }
    }
    private static void EnterBattle(MsgEnterBattle msg)
    {
        Reset();
        PanelManager.Close("RoomPanel");
        PanelManager.Close("ResultPanel");
        PanelManager.Open<TankUIPanel>();
        PanelManager.Open<PlayControlPanel>();
        for (int i = 0; i < msg.tanks.Length; i++)
        {
            GenerateTank(msg.tanks[i]);
        }
    }
    private static void GenerateTank(TankInfo tankInfo)
    {
        string objName = "Tank_" + tankInfo.name;
        GameObject tankObj = new GameObject(objName);

        BaseTank tank = null;
        if(tankInfo.id == GameMain.id)
        {
            tank = tankObj.AddComponent<CtrlTank>();
            tankObj.AddComponent<CameraFollow>();
        } else
        {
            tank = tankObj.AddComponent<SyncTank>();
        }
        tank.camp = tankInfo.camp;
        tank.id = tankInfo.id;
        tank.hp = tankInfo.hp;
        tank.tankName = tankInfo.name;
        Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
        Debug.Log(tankInfo.turretY);

        Debug.Log(tank.id + " pos => " + tankInfo.x + " " + tankInfo.y + " " + tankInfo.z);
        Debug.Log(tank.id + " rot => " + tankInfo.ex+" "+ tankInfo.ey + " " + tankInfo.ez);
        tank.transform.position = pos;
        tank.transform.eulerAngles = rot;
        //turret.localEulerAngles.
        if (tankInfo.camp == 1)
        {
            tank.Init("Prefabs/ModelPre/TankPrefab/tankPrefab");//todo 写入正确的坦克模型路径
        } else
        {
            //tank.Init("tankPrefab2");//todo 写入正确的坦克模型路径
            tank.Init("Prefabs/ModelPre/TankPrefab/tankPrefab");//todo 写入正确的坦克模型路径
        }
        AddTank(tankInfo.id, tank);
        tank.InitGunDirect(tankInfo.turretY);
    }
    private static void OnMsgBattleResult(MsgBase msgBase)
    {
        MsgBattleResult msg = (MsgBattleResult)msgBase;
        bool isWin = false;
        BaseTank tank = GetCtrlTank();
        if(tank != null && tank.camp == msg.winCamp)
        {
            isWin = true;
        }
        PanelManager.Open<ResultPanel>(isWin);//todo
        Reset();
    }
    private static  void OnMsgLeaveBattle(MsgBase msgBase)
    {
        MsgLeaveBattle msg = (MsgLeaveBattle)msgBase;
        BaseTank tank = GetTank(msg.id);
        if(tank == null)
        {
            return;
        }
        RemoveTank(msg.id);
        MonoBehaviour.Destroy(tank.gameObject);
    }
    public static void AddTank(int id, BaseTank tank)
    {
        tanks[id] = tank;
    }
    public static void RemoveTank(int id)
    {
        tanks.Remove(id);
    }
    public static BaseTank GetTank(int id)
    {
        if (tanks.ContainsKey(id))
        {
            return tanks[id];
        }
        return null;
    }
    public static BaseTank GetCtrlTank()
    {
        return GetTank(GameMain.id);
    }
    public static void Reset()
    {
        RemoveMsgListener();
        AddMsgListener();
        foreach (BaseTank tank in tanks.Values)
        {
            MonoBehaviour.Destroy(tank.gameObject);
        }
        tanks.Clear();
        PanelManager.Close("TankUIPanel");
        PanelManager.Close("PlayControlPanel");
    }
}
