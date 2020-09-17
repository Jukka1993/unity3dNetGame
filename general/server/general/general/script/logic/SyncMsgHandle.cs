using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void MsgSyncTank(ClientState cs, MsgBase msgBase)
        {
            MsgSyncTank msg = (MsgSyncTank)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            
        }
    }
}
