using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    class Room
    {
        public int roomId = 0;
        public int maxPlayer = 6;
        public Dictionary<int,bool> playerIds = new Dictionary<int,bool>();
        public int ownerId = -1;
        public enum Status
        {
            PREPARE = 0,
            FIGHT = 1
        }
        public Status status = Status.PREPARE;
        public bool AddPlayer(int id)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player == null)
            {
                Console.WriteLine("room.AddPlayer fail, player is null");
                return false;
            }
            if (playerIds.Count >= maxPlayer) {
                Console.WriteLine("room.AddPlayer fail, reach maxPlayer");
                return false;
            }
            if(status != Status.PREPARE)
            {
                Console.WriteLine("room.AddPlayer fail, not PREPARE");
                return false;
            }
            if (playerIds.ContainsKey(id))
            {
                Console.WriteLine("room.AddPlayer fail, already in this room");
                return false;
            }
            playerIds[id] = true;
            player.camp = SwitchCamp();
            player.roomId = this.roomId;
            if(ownerId == -1)
            {
                ownerId = player.id;
            }
            Broadcast(ToMsg());
            RoomManager.Broadcast();
            return true;
        }
        public bool RemovePlayer(int id)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player == null)
            {
                Console.WriteLine("room.RemovePlayer fail, player is null");
                return false;
            }
            if (!playerIds.ContainsKey(id))
            {
                Console.WriteLine("room.RemovePlayer fail, not in this room");
                return false;
            }
            playerIds.Remove(id);
            player.camp = 0;
            player.roomId = -1;
            if (isOwner(player))
            {
                ownerId = SwitchOwner();
            }
            if(playerIds.Count == 0)
            {
                RoomManager.RemoveRoom(this.roomId);
            } else
            {
                RoomManager.Broadcast();
            }
            Broadcast(ToMsg());
            return true;
        }
        public int SwitchCamp()
        {
            int count1 = 0;
            int count2 = 0;
            foreach(int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                if(player.camp == 1)
                {
                    count1++;
                }
                if(player.camp == 2)
                {
                    count2++;
                }
            }
            if(count1 <= count2)
            {
                return 1;
            } else
            {
                return 2;
            }
        }
        public bool isOwner(Player player)
        {
            return player.id == ownerId;
        }
        public int SwitchOwner()
        {
            foreach(int id in playerIds.Keys)
            {
                return id;
            }
            return -1;
        }
        public void Broadcast(MsgBase msg)
        {
            foreach(int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                player.Send(msg);
            }
        }
        public MsgBase ToMsg()
        {
            MsgGetRoomInfo msg = new MsgGetRoomInfo();
            int count = playerIds.Count;
            msg.players = new PlayerInfo[count];

            int i = 0;
            foreach(int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                PlayerInfo playerInfo = new PlayerInfo();

                playerInfo.name = player.name;
                playerInfo.id = player.id;
                playerInfo.camp = player.camp;
                playerInfo.winCount = player.data.winCount;
                playerInfo.lostCount = player.data.lostCount;
                playerInfo.isOwner = false;
                if (isOwner(player))
                {
                    playerInfo.isOwner = true;
                }
                msg.players[i] = playerInfo;
                i++;
            }
            return msg;
        }
    }
}
