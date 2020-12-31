using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;

namespace general.script.logic
{
    public enum PlayerState
    {
        OffLine,
        OutRoom,
        Preparing,
        Fighting
    }
    public class Player
    {
        public PlayerState status = PlayerState.OffLine;
        public int id = -1;
        public string name = "";
        public ClientState cs;
        public float x;
        public float y;
        public float z;
        public float ex;
        public float ey;
        public float ez;
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
            NetManager.Send(cs, msgBase);
        }
        public void ReEnterRoom()
        {
            Room room = RoomManager.GetRoom(roomId);
            //if (room)
            
            
        }
        public void BreakFromCS(bool breakConnect = true)
        {
            if (this.cs == null)
            {
                return;
            }
            ClientState oldCs = this.cs;
            this.cs = null;
            if (breakConnect)
            {
                NetManager.Close(oldCs);
            }
        }
        public void bindCS(ClientState newCs)
        {
            cs = newCs;
            lastPingTime = cs.lastPingTime;
        }
    }
}
