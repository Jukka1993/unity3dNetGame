using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Constant
{
    public enum RoomState
    {
        Preparing,
        Fighting
    }
    /// <summary>
    /// ping 的间隔, 每 pingInterval秒客户端发送一次ping,服务器收到ping后立刻返回pong
    /// </summary>
    public static int pingInterval = 2;
    /// <summary>
    /// pong等待次数,超过 pingInterval * pingWaitCount 的时间还没收到ping-pong信息,则认为玩家已经断开连接了
    /// </summary>
    public static int pongWaitCount = 4;
}

