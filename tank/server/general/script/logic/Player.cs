using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;

namespace general.script.logic
{
    public class Player
    {
        public int id = -1;
        public string name = "";
        public ClientState cs;
        public int x;
        public int y;
        public int z;
        public float ex;
        public float ey;
        public float ez;
        public int roomId = -1;
        public int camp = 1;
        public int hp = 100;
        public PlayerData data;
        public Player(ClientState state)
        {
            this.cs = state;
        }
        public void Send(MsgBase msgBase)
        {
            NetManager.Send(cs, msgBase);
        }
    }
}
