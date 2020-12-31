using System;
using System.Collections.Generic;
using System.Text;


public static class Constant
{
    /// <summary>
    /// ping的间隔,每 pingInterval 毫秒将ping一次
    /// </summary>
    public static long pingInterval = 30000;
    /// <summary>
    /// ping等待次数,超过 pingInterval * pingWaitCount 的时间还没收到ping-pong信息,则认为玩家已经断开连接了
    /// </summary>
    public static int pingWaitCount = 4;
    /// <summary>
    /// 当房间处于准备状态时,超过 pingInterval * preparingRoomWaitCount 的时间还没收到ping-pong信息,则可以将这个玩家踢出房间了
    /// </summary>
    public static int preparingRoomWaitCount = 20;
}
