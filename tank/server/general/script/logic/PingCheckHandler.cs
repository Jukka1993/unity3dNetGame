using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void OnReceivePingCheckMsg(ClientState cs, MsgBase msgBase)
        {
            PingCheckMsg msg = (PingCheckMsg)msgBase;
            msg.serverReceiveTime2 = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            Player player = cs.player;
            if (player == null)
            {
                return;
            }
            msg.serverSendTime3 = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            player.Send(msg);
        }
    }
}
