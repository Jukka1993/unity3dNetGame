using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void OnReceiveMsgGetAchieve(ClientState cs, MsgBase msgBase)
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
        public static void OnReceiveMsgGetRoomList(ClientState cs, MsgBase msgBase)
        {

            MsgGetRoomList msg = (MsgGetRoomList)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            player.Send(RoomManager.ToMsg());
        }
        public static void OnReceiveMsgCreateRoom(ClientState cs, MsgBase msgBase)
        {
            //CommonUtil.Log(cs.)
            MsgCreateRoom msg = (MsgCreateRoom)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString() + "的创建房间请求,其没有player，不予处理");
                return;
            }
            if(player.roomId >= 0)
            {
                msg.roomId = -1;
                player.Send(msg);
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString() + "的创建房间请求,其player本来就在房间里，返回失败");
                return;
            }
            Room room = RoomManager.AddRoom();
            room.AddPlayer(player.id);
            msg.roomId = room.roomId;
            player.Send(msg);

            CommonUtil.Log("玩家 " + player.id + " 创建并进入房间 " + msg.roomId);
        }
        public static void OnReceiveMsgEnterRoom(ClientState cs, MsgBase msgBase)
        {
            MsgEnterRoom msg = (MsgEnterRoom)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString() + "的进入房间请求,其没有player，不予处理");

                return;
            }
            if(player.roomId >= 0)
            {
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString() + "的进入房间请求,其player本来就在房间里，返回失败");
                msg.result = 0;
                player.Send(msg);
                return;
            }
            Room room = RoomManager.GetRoom(msg.id);
            if(room == null)
            {
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString() + "的进入房间",msg.id.ToString(),"的请求,该房间不存在，返回失败");
                msg.result = 1;
                player.Send(msg);
                return;
            }
            if (!room.AddPlayer(player.id))
            {
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString() + "的进入房间", msg.id.ToString(), "的请求,但进入失败，返回失败");

                msg.result = 1;
                player.Send(msg);
                return;
            }
            msg.result = 0;
            player.Send(msg);
        }
        public static void OnReceiveMsgGetRoomInfo(ClientState cs, MsgBase msgBase)
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
        public static void OnReceiveMsgKickPlayer(ClientState cs, MsgBase msgBase)
        {
            MsgKickPlayer msg = (MsgKickPlayer)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString(), "的踢人请求,但其没有绑定player，不予处理");
                return;
            }
            msg.kickPlayerId = player.id;
            Room room = RoomManager.GetRoom(cs.player.roomId);
            if(room == null)
            {
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString(), "的踢人请求,但其player不在room中，返回失败");
                msg.result = 1;
                NetManager.Send(cs, msg);
                return;
            }
            if(room.ownerId != player.id)
            {//不是房主无权踢人
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString(), "的踢人请求,但其player"+player.id+"不是房主，无权踢人，返回失败");
                msg.result = 1;
                NetManager.Send(cs, msg);
                return;
            }
            if(player.id == msg.kickedPlayerId)
            {//不能踢自己
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString(), "的踢人请求,但其player" + player.id + "不能把自己踢走，返回失败");

                msg.result = 1;
                NetManager.Send(cs, msg);
                return;
            }
            if(room.playerIds.ContainsKey(msg.kickedPlayerId) == false)
            {//要踢的人在房间里不存在
                CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString(), "的踢人请求,但其player" + player.id + "要踢的id为",msg.kickedPlayerId.ToString(),"的player不在房间中，返回失败");
                msg.result = 1;
                NetManager.Send(cs, msg);
                return;
            }
            CommonUtil.Log("收到", cs.socket.RemoteEndPoint.ToString(), "的踢人请求，player" + player.id + "要踢的id为",msg.kickedPlayerId.ToString(),"成功从", room.roomId.ToString(), "踢出");
            room.RemovePlayer(msg.kickedPlayerId);
            msg.result = 0;
            Player kickedPlayer = PlayerManager.GetPlayer(msg.kickedPlayerId);
            kickedPlayer.Send(msg);
            player.Send(msg);
        }
        public static void OnReceiveMsgLeaveRoom(ClientState cs, MsgBase msgBase)
        {
            MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 离开房间 失败,没有绑定的player");

                return;
            }
            int id = -1;
            Room room = RoomManager.GetRoom(player.roomId);
            if(room == null)
            {
                msg.result = 1;
                player.Send(msg);
                CommonUtil.Log("玩家 " + player.id + " 离开房间 " + id,"失败,本来就不在房间内");

                return;
            }
            id = room.roomId;
            room.RemovePlayer(player.id);
            msg.result = 0;
            player.Send(msg);
            CommonUtil.Log("玩家 " + player.id + " 离开房间 " + id.ToString(),"成功");
        }
        public static void OnReceiveMsgStartBattle(ClientState cs, MsgBase msgBase)
        {
            MsgStartBattle msg = (MsgStartBattle)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                CommonUtil.Log(cs.socket.RemoteEndPoint.ToString(), "尝试开始战斗失败，因为它没有player");
                return;
            }
            Room room = RoomManager.GetRoom(player.roomId);
            if(room == null)
            {
                CommonUtil.Log(player.id.ToString(), "尝试开始战斗失败，因为它不在房间内");
                msg.result = 1;
                player.Send(msg);
                return;
            }
            if (!room.isOwner(player))
            {
                CommonUtil.Log(player.id.ToString(), "尝试开始战斗失败，因为它不是房主");
                msg.result = 1;
                player.Send(msg);
                return;
            }
            if (!room.StartBattle())
            {
                CommonUtil.Log(player.id.ToString(), "尝试开始战斗失败");
                msg.result = 1;
                player.Send(msg);
                return;
            }
            CommonUtil.Log(player.id.ToString(), "尝试开始战斗成功");
            msg.result = 0;
            player.Send(msg);
        }
    }
}
