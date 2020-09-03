using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using general.script.logic;

namespace general.script.net
{
    public class ClientState
    {
        public Socket socket;
        public ByteArray readBuff = new ByteArray();
        //ping
        public long lastPingTime = 0;
        //玩家
        public Player player;
    }
}
