using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace general.script.net
{
    public class ClientState
    {
        public Socket socket;
        public ByteArray readBuff = new ByteArray();
        //玩家数据后面添加
        public long lastPingTime = 0;
    }
}
