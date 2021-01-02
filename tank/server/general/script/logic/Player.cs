using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public enum PlayerState
    {
        OutRoom,
        Preparing,
        Fighting
    }
    public class Player
    {
        public bool Connected
        {
            get
            {
                return cs == null;
            }
        }
        public PlayerState status = PlayerState.OutRoom;
        public int id = -1;
        public string name = "";
        public ClientState cs;
        public float x;
        public float y;
        public float z;
        public float ex;
        public float ey;
        public float ez;
        public float turretY;
        public int roomId = -1;
        public int camp = 1;
        public int hp = 100;
        public long lastPingTime = 0;
        public PlayerData data;
        public Player(ClientState state)
        {
            cs = state;
            status = PlayerState.OutRoom;
            lastPingTime = cs.lastPingTime;
        }
        public void Send(MsgBase msgBase)
        {
            if (cs == null)
            {
                return;
            }
            NetManager.Send(cs, msgBase);
        }
        public void BreakFromCS(bool breakConnect = true, bool isNormalBreak = true)
        {
            if (cs == null)
            {
                return;
            }
            ClientState oldCs = this.cs;
            this.cs = null;
            if (breakConnect)
            {
                NetManager.Close(oldCs, isNormalBreak);
            }
        }
        public void UnBindCS()
        {
            ClientState oldCs = cs;
            cs = null;
            if (oldCs != null)
            {
                NetManager.Close(oldCs, false);
            }
            NotifyRoom();
        }
        public void BindCS(ClientState newCs)
        {
            cs = newCs;
            lastPingTime = cs.lastPingTime;
            NotifyRoom();
        }
        public void NotifyRoom()
        {
            if (roomId >= 0)
            {
                Room room = RoomManager.GetRoom(roomId);
                if (room != null)
                {
                    room.PlayerStatusChange();
                }
            }
        }
    }
}
