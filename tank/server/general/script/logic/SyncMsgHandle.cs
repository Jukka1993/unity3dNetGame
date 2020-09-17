using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void MsgSyncTank(ClientState cs, MsgBase msgBase)
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
            if(room.status != Room.Status.FIGHT)
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
            player.ex = msg.x;
            player.ey = msg.y;
            player.ez = msg.z;
            msg.id = player.id; //填充id
            //广播
            room.Broadcast(msg);
        }
        public static void MsgFire(ClientState cs, MsgBase msgBase)
        {
            MsgFire msg = (MsgFire)msgBase;
            Player p = cs.player;
            if(p== null)
            {
                return;
            }
            Room room = RoomManager.GetRoom(p.id);
            if(room == null)
            {
                return;
            }
            if(room.status != Room.Status.FIGHT)
            {
                return;
            }
            msg.id = p.id;
            room.Broadcast(msg);
        }
        public static void MsgHit(ClientState cs,MsgBase msgBase)
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
            if(room.status != Room.Status.FIGHT)
            {
                return;
            }
            if(player.id != msg.id)
            {
                return;
            }
            int damage = 35;
            targetPlayer.hp -= damage;

            msg.id = player.id;
            msg.hp = player.hp;
            msg.damage = damage;
            room.Broadcast(msg);

        }
    }
}

