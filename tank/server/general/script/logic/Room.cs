using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    class Room
    {
        static float[,,] birthConfig = new float[2, 3, 6]
        {
            {//阵营1
                { 1f,2f,3f,4f,5f,6f},     //阵营1,1号出生点
                { 1f,2f,3f,4f,5f,6f},     //阵营1,2号出生点
                { 1f,2f,3f,4f,5f,6f}      //阵营1,3号出生点
            },
            {//阵营2
                { 13f,4f,13f,4f,5f,6f},     //阵营2,1号出生点
                { 11f,2f,13f,4f,5f,6f},     //阵营2,2号出生点
                { 11f,2f,13f,4f,5f,6f}      //阵营2,3号出生点
            }
        };
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
        private long lastJudgeTime = 0;
        public void Update()
        {
            if(status != Status.FIGHT)
            {
                return;
            }
            if(NetManager.GetTimeStamp() - lastJudgeTime < 10f)
            {
                return;
            }
            lastJudgeTime = NetManager.GetTimeStamp();
            int winCamp = Judgement();
            if(winCamp == 0)
            {
                return;
            }
            status = Status.PREPARE;
            foreach(int id in playerIds.Keys)
            {
                Player p = PlayerManager.GetPlayer(id);
                if(p.camp == winCamp)
                {
                    p.data.winCount++;
                } else
                {
                    p.data.lostCount++;
                }
            }
            MsgBattleResult msg = new MsgBattleResult();
            msg.winCamp = winCamp;
            Broadcast(msg);
        }
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
            if(status == Status.FIGHT)
            {
                player.data.lostCount++;
                MsgLeaveBattle msg = new MsgLeaveBattle();
                msg.id = player.id;
                Broadcast(msg);
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
        public bool StartBattle()
        {
            if (!CanStartBattle())
            {
                return false;
            }
            status = Status.FIGHT;
            ResetPlayers();
            MsgEnterBattle msg = new MsgEnterBattle();
            msg.mapId = 1;
            msg.tanks = new TankInfo[playerIds.Count];
            int i = 0;
            foreach(int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                msg.tanks[i] = PlayerToTankInfo(player);
                i++;
            }
            Broadcast(msg);
            return true;
        }
        public bool IsDie(Player p)
        {
            return p.hp <= 0;
        }
        public int Judgement()
        {
            int count1 = 0;
            int count2 = 0;
            foreach (int id in playerIds.Keys)
            {
                Player p = PlayerManager.GetPlayer(id);
                if (!IsDie(p))
                {
                    if (p.camp == 1)
                    {
                        count1++;
                    }
                    if (p.camp == 2)
                    {
                        count2++;
                    }
                }
            }
            if (count1 <= 0)
            {
                return 2;
            }
            else if (count2 <= 0)
            {
                return 1;
            }
            return 0;
        }
        public TankInfo PlayerToTankInfo(Player player)
        {
            TankInfo info = new TankInfo();
            info.camp = player.camp;
            info.id = player.id;
            info.hp = player.hp;
            info.x = player.x;
            info.y = player.y;
            info.z = player.z;
            info.ex = player.ex;
            info.ey = player.ey;
            info.ez = player.ez;
            return info;
        }
        public bool CanStartBattle()
        {
            if(status != Status.PREPARE)
            {
                return false;
            }
            int count1 = 0;
            int count2 = 0;
            foreach(int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                if(player.camp == 1)
                {
                    count1++;
                } else
                {
                    count2++;
                }
            }
            if(count1 <1 || count2 < 1)
            {
                return false;
            }
            return true;
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
        private void SetBirthPos(Player player,int index)
        {
            int camp = player.camp;
            player.x = birthConfig[camp - 1, index, 0];
            player.y = birthConfig[camp - 1, index, 1];
            player.z = birthConfig[camp - 1, index, 2];
            player.ex = birthConfig[camp - 1, index, 3];
            player.ey = birthConfig[camp - 1, index, 4];
            player.ez = birthConfig[camp - 1, index, 5];
        }
        private void ResetPlayers()
        {
            int count1 = 0;
            int count2 = 0;
            foreach(int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                if(player.camp == 1)
                {
                    SetBirthPos(player, count1);
                    count1++;
                } else
                {
                    SetBirthPos(player, count2);
                    count2++;
                }
                player.hp = 100;
            }
        }
    }
}
