using System;
using System.Collections.Generic;
using System.Text;


public static class Constant
{
    public static string dbName = "netgame";
    public static string dbIP = "127.0.0.1";
    //public static int dbPort = 13306;
    public static int dbPort = 3306;
    public static string dbLoginAccount = "root";
    public static string dbPassWord = "199349529";
    /// <summary>
    /// ping的间隔,预计每 pingInterval 毫秒将收到客户端发来的ping一次 与客户端保持一致
    /// </summary>
    public static long pingInterval = 2000;
    /// <summary>
    /// ping等待次数,超过 pingInterval * pingWaitCount 的时间还没收到ping-pong信息,则认为玩家已经断开连接了
    /// </summary>
    public static int pingWaitCount = 4;
    /// <summary>
    /// 当房间处于准备状态时,超过 pingInterval * preparingRoomWaitCount 的时间还没收到ping-pong信息,则可以将这个玩家踢出房间了
    /// </summary>
    public static int preparingRoomWaitCount = 20;
    public enum RoomState
    {
        Preparing,
        Fighting
    }
    public static bool showLog = true;
    public static string[] filterProtoNames = { "PingCheckMsg", "" };
    public static bool filterAllProto = true;
}
