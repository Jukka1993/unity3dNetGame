using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.db;

namespace general.script.logic
{
    public partial class EventHandler
    {
        public static void OnDisconnect(ClientState cs)
        {
            Console.WriteLine("Close111");
            //Player下线
            if(cs.player != null)
            {
                if(cs.player != null)
                {
                    int roomId = cs.player.roomId;
                    if(roomId >= 0)
                    {
                        Room room = RoomManager.GetRoom(roomId);
                        room.RemovePlayer(cs.player.id);
                    }
                }

                //保存数据
                DBManager.UpdatePlayerData(cs.player.id, cs.player.data);
                //移除
                PlayerManager.RemovePlayer(cs.player.id);
            }
        }
        public static void OnTimer()
        {
            CheckPing();
        }
        public static void CheckPing()
        {
            long timeNow = NetManager.GetTimeStamp();
            long overtime = NetManager.pingInterval * 4;
            foreach (ClientState cs in NetManager.clients.Values)
            {
                if(timeNow -cs.lastPingTime > overtime)
                {
                    Console.WriteLine("Ping Close " + cs.socket.RemoteEndPoint.ToString());
                    NetManager.Close(cs);
                    return;
                }
            }
        }
    }
}
