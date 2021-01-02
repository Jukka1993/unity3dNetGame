using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void OnReceiveMsgSyncTank(ClientState cs, MsgBase msgBase)
        {
            MsgSyncTank msg = (MsgSyncTank)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            Room room = RoomManager.GetRoom(player.roomId);
            if(room == null)
            {
                return;
            }
            if(room.status != Constant.RoomState.Fighting)
            {
                return;
            }
            if(Math.Abs(player.x - msg.x) > 5
                ||Math.Abs(player.y - msg.y) > 5
                  ||  Math.Abs(player.z - msg.z) > 5
                )
            {
                Console.WriteLine("疑似作弊 " + player.id);
            }
            player.x = msg.x;
            player.y = msg.y;
            player.z = msg.z;
            player.ex = msg.ex;
            player.ey = msg.ey;
            player.ez = msg.ez;
            player.turretY = msg.turretY;
            msg.id = player.id; //填充id
            //广播
            room.Broadcast(msg);
        }
        public static void OnReceiveMsgFire(ClientState cs, MsgBase msgBase)
        {
            MsgFire msg = (MsgFire)msgBase;
            Player p = cs.player;
            if(p== null)
            {
                Console.WriteLine("receive firemsg,but player is not exist");

                return;
            }
            Room room = RoomManager.GetRoom(p.roomId);
            if(room == null)
            {
                Console.WriteLine("receive firemsg,but room is null");

                return;
            }
            if(room.status != Constant.RoomState.Fighting)
            {
                Console.WriteLine("receive firemsg,but room.status is prepare");
                return;
            }
            Console.WriteLine("{0} fire,x={1},y={2},z={3},ex={4},ey={5},ez={6}", msg.id, msg.x, msg.y, msg.z, msg.ex, msg.ey, msg.ez);
            msg.id = p.id;
            room.Broadcast(msg,true);
        }
        public static void OnReceiveMsgHit(ClientState cs,MsgBase msgBase)
        {
            MsgHit msg = (MsgHit)msgBase;
            Player player = cs.player;
            if (player == null)
            {
                return;
            }
            Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
            if(targetPlayer == null)
            {
                return;
            }
            Room room = RoomManager.GetRoom(player.roomId);
            if(room == null)
            {
                return;
            }
            if(room.status != Constant.RoomState.Fighting)
            {
                return;
            }
            if(player.id != msg.id)
            {
                return;
            }
            int damage = 10;
            targetPlayer.hp -= damage;

            msg.id = player.id;
            msg.hp = player.hp;
            msg.damage = damage;
            room.Broadcast(msg);

        }
    }
}

