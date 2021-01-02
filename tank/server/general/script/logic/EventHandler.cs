using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.db;

namespace general.script.logic
{
    public partial class EventHandler
    {
        public static void OnDisconnect(ClientState cs, bool isNormalDisConnect)
        {
            //Player下线
            if (cs.player != null)
            {
                if (isNormalDisConnect)
                {
                    int roomId = cs.player.roomId;
                    if (roomId >= 0)
                    {
                        Room room = RoomManager.GetRoom(roomId);
                        {
                            room.RemovePlayer(cs.player.id);
                        }
                    }
                    //保存数据
                    DBManager.UpdatePlayerData(cs.player.id, cs.player.data);
                    //移除
                    PlayerManager.RemovePlayer(cs.player.id);//todo 断开连接不要立刻删除player了,如果是战斗中,就一直保留,如果是在房间中,则等待一段时间,如果还没有连接回来,删除player
                }
                else
                {
                    cs.player.UnBindCS();

                }
            }
        }
        public static void OnTimer()
        {
            CheckPing();
            RoomManager.Update();
        }
        public static void CheckPing()
        {
            long timeNow = NetManager.GetTimeStamp();
            long overtime = Constant.pingInterval * Constant.pingWaitCount;
            foreach (ClientState cs in NetManager.clients.Values)
            {
                if (timeNow - cs.lastPingTime > overtime)
                {
                    Console.WriteLine("Ping Close " + cs.socket.RemoteEndPoint.ToString());
                    NetManager.Close(cs,false);
                    return;
                }
            }
        }
    }
}
