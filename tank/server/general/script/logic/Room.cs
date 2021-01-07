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
                { 10f,2f,30f,0f,0f,0f},     //阵营1,1号出生点
                { 10f,2f,0f,0f,0f,0f},     //阵营1,2号出生点
                { 10f,2f,-30f,0f,0f,0f}      //阵营1,3号出生点
            },
            {//阵营2
                { -10f,2f,30f,0f,0f,0f},     //阵营2,1号出生点
                { -10f,2f,0f,0f,0f,0f},     //阵营2,2号出生点
                { -10f,2f,-30f,0f,0f,0f}      //阵营2,3号出生点
            }
        };
        public int roomId = 0;
        public int maxPlayer = 6;
        public Dictionary<int, bool> playerIds = new Dictionary<int, bool>();
        public int ownerId = -1;
        public Constant.RoomState status = Constant.RoomState.Preparing;
        private long lastJudgeTime = 0;
        public void Update()
        {
            if (status == Constant.RoomState.Fighting)
            {
                FightingUpdate();
                return;
            }
            if (status == Constant.RoomState.Preparing)
            {
                PreparingUpdate();
                return;
            }
        }
        public bool AddPlayer(int id)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (player == null)
            {
                CommonUtil.Log("player", id.ToString(), "不存在,无法加入房间", roomId.ToString());
                return false;
            }
            if (playerIds.Count >= maxPlayer)
            {
                CommonUtil.Log("player", id.ToString(), "无法加入房间", roomId.ToString(),"因为房间已满");
                return false;
            }
            if (status != Constant.RoomState.Preparing)
            {
                CommonUtil.Log("player", id.ToString(), "无法加入房间", roomId.ToString(), "因为房间在战斗中");
                return false;
            }
            if (playerIds.ContainsKey(id))
            {
                CommonUtil.Log("player", id.ToString(), "无法加入房间", roomId.ToString(), "因为它本身就在房间里了");
                return false;
            }

            playerIds[id] = true;
            player.camp = SwitchCamp();
            player.roomId = roomId;
            if (ownerId == -1)
            {
                ownerId = player.id;
            }
            Broadcast(ToMsg());
            RoomManager.Broadcast();
            CommonUtil.Log("player", id.ToString(), "加入了房间", roomId.ToString(), "其阵营为", player.camp.ToString());
            return true;
        }

        public bool RemovePlayer(int id)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (player == null)
            {
                CommonUtil.Log("player", id.ToString(), "移出房间", roomId.ToString(), "失败，因为它不存在");

                return false;
            }
            if (!playerIds.ContainsKey(id))
            {
                CommonUtil.Log("player", id.ToString(), "移出房间", roomId.ToString(), "失败，因为它本来就不在该房间");
                return false;
            }
            playerIds.Remove(id);
            player.camp = 0;
            player.roomId = -1;
            
            //if(status == Status.FIGHT) //战斗中就不踢出人了,断网了,就让坦克在房间不动就好了
            //{
            //    player.data.lostCount++;
            //    MsgLeaveBattle msg = new MsgLeaveBattle();
            //    msg.id = player.id;
            //    Broadcast(msg);
            //}

            if (playerIds.Count == 0)
            {
                CommonUtil.Log("player", id.ToString(), "移出房间", roomId.ToString(), "成功");

                RoomManager.RemoveRoom(this.roomId);
            }
            else
            {
                if (isOwner(player))
                {
                    ownerId = SwitchOwner();
                    CommonUtil.Log("player", id.ToString(), "移出房间", roomId.ToString(), "成功,新房主id为", ownerId.ToString());
                }
                RoomManager.Broadcast();
            }
            Broadcast(ToMsg());
            return true;
        }
        /// <summary>
        /// 玩家状态变化了
        /// </summary>
        public void PlayerStatusChange()
        {
            Broadcast(ToMsg());
        }
        public int SwitchCamp()
        {
            int count1 = 0;
            int count2 = 0;
            foreach (int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                if (player.camp == 1)
                {
                    count1++;
                }
                if (player.camp == 2)
                {
                    count2++;
                }
            }
            if (count1 <= count2)
            {
                return 1;
            }
            else
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
            foreach (int id in playerIds.Keys)
            {
                return id;
            }
            return -1;
        }
        public void Broadcast(MsgBase msg, bool showLog = false)
        {
            foreach (int id in playerIds.Keys)
            {
                if (showLog)
                {
                    Console.WriteLine("should send fire msg to ${0}", id);
                }
                Player player = PlayerManager.GetPlayer(id);
                player.Send(msg);
            }
        }
        public void ReEnterRoom(Player player)
        {
            //if (status == Constant.RoomState.Fighting)
            //{
            MsgReEnterRoom msg = new MsgReEnterRoom();
            msg.roomId = roomId;
            msg.roomState = status;

            if (status == Constant.RoomState.Fighting)
            {
                msg.mapId = 1;
                msg.tanks = new TankInfo[playerIds.Count];
                int i = 0;
                foreach (int id in playerIds.Keys)
                {
                    Player playerTemp = PlayerManager.GetPlayer(id);
                    msg.tanks[i] = PlayerToTankInfo(playerTemp);
                    i++;
                }
            }

            player.Send(msg);
            if (status == Constant.RoomState.Preparing)
            {
                player.Send(ToMsg());
            }
            //} else if (status == Constant.RoomState.Preparing)
            //{
            //    
            //}
        }
        public bool StartBattle()
        {
            if (!CanStartBattle())
            {
                return false;
            }
            status = Constant.RoomState.Fighting;
            ResetPlayers();
            MsgEnterBattle msg = new MsgEnterBattle();
            msg.mapId = 1;
            msg.tanks = new TankInfo[playerIds.Count];
            int i = 0;
            foreach (int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                msg.tanks[i] = PlayerToTankInfo(player);
                i++;
            }
            CommonUtil.Log("开始战斗成功并广播");
            Broadcast(msg);
            return true;
        }
        public bool IsDie(Player p)
        {
            return p.hp <= 0;
        }

        public TankInfo PlayerToTankInfo(Player player)
        {
            TankInfo info = new TankInfo();
            info.name = player.name;
            info.camp = player.camp;
            info.id = player.id;
            info.hp = player.hp;
            info.x = player.x;
            info.y = player.y;
            info.z = player.z;
            info.ex = player.ex;
            info.ey = player.ey;
            info.ez = player.ez;
            info.turretY = player.turretY;
            return info;
        }
        public bool CanStartBattle()
        {
            if (status != Constant.RoomState.Preparing)
            {
                return false;
            }
            int count1 = 0;
            int count2 = 0;
            foreach (int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                if (player.camp == 1)
                {
                    count1++;
                }
                else
                {
                    count2++;
                }
            }
            if (count1 < 1 || count2 < 1)
            {
                CommonUtil.Log("开始战斗失败");
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
            foreach (int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                PlayerInfo playerInfo = new PlayerInfo();

                playerInfo.name = player.name;
                playerInfo.id = player.id;
                playerInfo.camp = player.camp;
                playerInfo.winCount = player.data.winCount;
                playerInfo.lostCount = player.data.lostCount;
                playerInfo.isOwner = false;
                playerInfo.connected = player.Connected;
                if (isOwner(player))
                {
                    playerInfo.isOwner = true;
                }
                msg.players[i] = playerInfo;
                i++;
            }
            return msg;
        }
        private void SetBirthPos(Player player, int index)
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
            foreach (int id in playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                if (player.camp == 1)
                {
                    SetBirthPos(player, count1);
                    count1++;
                }
                else
                {
                    SetBirthPos(player, count2);
                    count2++;
                }
                player.hp = 100;
            }
        }
        private void FightingUpdate()
        {
            if (NetManager.GetTimeStamp() - lastJudgeTime < 10f)
            {
                return;
            }
            lastJudgeTime = NetManager.GetTimeStamp();
            int winCamp = Judgement();
            if (winCamp == 0)
            {
                return;
            }
            status = Constant.RoomState.Preparing;
            foreach (int id in playerIds.Keys)
            {
                Player p = PlayerManager.GetPlayer(id);
                if (p.camp == winCamp)
                {
                    p.data.winCount++;
                }
                else
                {
                    p.data.lostCount++;
                }
            }
            MsgBattleResult msg = new MsgBattleResult();
            msg.winCamp = winCamp;
            Broadcast(msg);
        }
        private void PreparingUpdate()
        {
            int[] keys = new int[playerIds.Keys.Count];
            playerIds.Keys.CopyTo(keys, 0);
            long timestamp = NetManager.GetTimeStamp();

            foreach (int id in keys)
            {
                Player player = PlayerManager.GetPlayer(id);
                if (timestamp - player.lastPingTime > Constant.pingInterval * Constant.preparingRoomWaitCount)
                {
                    CommonUtil.Log("玩家", player.id.ToString(), "在房间", roomId.ToString(), "内离线时间太久,移出房间");
                    RemovePlayer(id);//todo 房间准备状态,若玩家离线太久,则踢出房间
                }
            }
        }
        private int Judgement()
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
                CommonUtil.Log("房间", roomId.ToString(), "中,阵营2胜利");
                return 2;
            }
            else if (count2 <= 0)
            {
                CommonUtil.Log("房间", roomId.ToString(), "中,阵营1胜利");
                return 1;
            }
            return 0;
        }
    }
}
