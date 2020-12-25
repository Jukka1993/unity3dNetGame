using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PingCheckMsg:MsgBase
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

