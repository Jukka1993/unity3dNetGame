using System.Collections;
using System.Collections.Generic;

using general.script.net;
namespace general.script.proto
{
    public class MsgPing : MsgBase
    {
        public MsgPing() { protoName = "MsgPing"; }
    }
    public class MsgPong : MsgBase
    {
        public MsgPong() { protoName = "MsgPong"; }
    }
}