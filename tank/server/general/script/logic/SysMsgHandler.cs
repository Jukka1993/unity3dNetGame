using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void OnReceiveMsgPing(ClientState cs, MsgBase msg)
        {
            //Console.WriteLine("Receive MsgPing");
            cs.lastPingTime = NetManager.GetTimeStamp();
            if (cs.player != null)
            {
                cs.player.lastPingTime = cs.lastPingTime;
            }
            MsgPong msgPong = new MsgPong();
            NetManager.Send(cs, msgPong);
        }
    }
}
