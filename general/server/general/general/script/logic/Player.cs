using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;

namespace general.script.logic
{
    public class Player
    {
        public int id = -1;
        public ClientState cs;
        public int x;
        public int y;
        public int z;
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
