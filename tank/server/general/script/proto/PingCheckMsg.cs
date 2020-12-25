using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
namespace general.script.proto
{
    class PingCheckMsg: MsgBase
    {
        public PingCheckMsg()
        {
            protoName = "PingCheckMsg";
        }
        public double clientSendTime1 = 0;
        public double serverReceiveTime2 = 0;
        public double serverSendTime3 = 0;
        public double clientReceiveTime4 = 0;
    }
}
