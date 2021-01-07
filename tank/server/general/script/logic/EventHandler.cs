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
                CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 断开连接,正常断开,同时将与其绑定的 player " + cs.player.id + " 移除");

                if (isNormalDisConnect)
                {
                    int roomId = cs.player.roomId;
                    if (roomId >= 0)
                    {
                        Room room = RoomManager.GetRoom(roomId);
                        {
                            CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 断开连接,正常断开,同时将与其绑定的 player(在房间"+roomId+"内） " + cs.player.id + " 移除");
                            room.RemovePlayer(cs.player.id);
                        }
                    }
                    else
                    {
                        CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 断开连接,正常断开,同时将与其绑定的 player(不在房间内） " + cs.player.id + " 移除");
                    }
                    //保存数据
                    DBManager.UpdatePlayerData(cs.player.id, cs.player.data);
                    //移除
                    PlayerManager.RemovePlayer(cs.player.id);//todo 断开连接不要立刻删除player了,如果是战斗中,就一直保留,如果是在房间中,则等待一段时间,如果还没有连接回来,删除player
                }
                else
                {
                    CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 断开连接,异常断开,将其与连接的 player "+ cs.player.id+ " 解绑");
                    cs.player.UnBindCS();
                }
            } else
            {
                CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 断开连接,本身没有player，无需处理player");
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
                    CommonUtil.Log("Ping-Pong超时,关闭连接 " + cs.socket.RemoteEndPoint.ToString());
                    NetManager.Close(cs,false);
                    return;
                }
            }
        }
    }
}
