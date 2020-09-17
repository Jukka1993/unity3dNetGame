using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void MsgGetAchieve(ClientState cs, MsgBase msgBase)
        {
            MsgGetAchieve msg = (MsgGetAchieve)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            msg.winCount = player.data.winCount;
            msg.failCount = player.data.lostCount;
            player.Send(msg);
        }
        public static void MsgGetRoomList(ClientState cs, MsgBase msgBase)
        {

            MsgGetRoomList msg = (MsgGetRoomList)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            player.Send(RoomManager.ToMsg());
        }
        public static void MsgCreateRoom(ClientState cs, MsgBase msgBase)
        {
            MsgCreateRoom msg = (MsgCreateRoom)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            if(player.roomId >= 0)
            {
                msg.roomId = -1;
                player.Send(msg);
                return;
            }
            Room room = RoomManager.AddRoom();
            room.AddPlayer(player.id);
            msg.roomId = room.roomId;
            player.Send(msg);
        }
        public static void MsgEnterRoom(ClientState cs, MsgBase msgBase)
        {
            MsgEnterRoom msg = (MsgEnterRoom)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            if(player.roomId >= 0)
            {
                msg.result = 0;
                player.Send(msg);
                return;
            }
            Room room = RoomManager.GetRoom(msg.id);
            if(room == null)
            {
                msg.result = 1;
                player.Send(msg);
                return;
            }
            if (!room.AddPlayer(player.id))
            {
                msg.result = 1;
                player.Send(msg);
                return;
            }
            msg.result = 0;
            player.Send(msg);
        }
        public static void MsgGetRoomInfo(ClientState cs, MsgBase msgBase)
        {
            MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            Room room = RoomManager.GetRoom(player.roomId);
            if(room == null)
            {
                player.Send(msg);
                return;
            }
            player.Send(room.ToMsg());
        }
        public static void MsgLeaveRoom(ClientState cs, MsgBase msgBase)
        {
            MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            Room room = RoomManager.GetRoom(player.roomId);
            if(room == null)
            {
                msg.result = 1;
                player.Send(msg);
                return;
            }
            room.RemovePlayer(player.id);
            msg.result = 0;
            player.Send(msg);
        }
        public static void MsgStartBattle(ClientState cs, MsgBase msgBase)
        {
            MsgStartBattle msg = (MsgStartBattle)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            Room room = RoomManager.GetRoom(player.id);
            if(room == null)
            {
                msg.result = 1;
                player.Send(msg);
                return;
            }
            if (!room.isOwner(player))
            {
                msg.result = 1;
                player.Send(msg);
                return;
            }
            if (!room.CanStartBattle())
            {
                msg.result = 1;
                player.Send(msg);
                return;
            }
            msg.result = 0;
            player.Send(msg);
        }
    }
}
