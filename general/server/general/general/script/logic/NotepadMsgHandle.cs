﻿using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;
namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void MsgGetText(ClientState cs, MsgBase msgBase)
        {
            MsgGetText msg = (MsgGetText)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            msg.text = player.data.text;
            player.Send(msg);
        }
        public static void MsgSaveText(ClientState cs, MsgBase msgBase)
        {
            MsgSaveText msg = (MsgSaveText)msgBase;
            Player player = cs.player;
            if(player == null)
            {
                return;
            }
            player.data.text = msg.text;
            player.Send(msg);
        }
    }
}
