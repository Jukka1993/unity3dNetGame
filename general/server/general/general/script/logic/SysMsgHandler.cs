using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void MsgPing(ClientState cs, MsgBase msg)
        {
            Console.WriteLine("Receive MsgPing");
            cs.lastPingTime = NetManager.GetTimeStamp();
            MsgPong msgPong = new MsgPong();
            NetManager.Send(cs, msgPong);
        }
    }
}
