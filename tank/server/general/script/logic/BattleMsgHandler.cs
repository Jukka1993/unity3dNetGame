using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;
namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void MsgMove(ClientState cs,MsgBase msgBase)
        {
            MsgMove msgMove = (MsgMove)msgBase;
            Console.WriteLine(msgMove.x);
            msgMove.x++;
            msgMove.y--;
            msgMove.z = 0;
            NetManager.Send(cs, msgMove);
        }
    }

}
