using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
namespace general.script.proto
{
    public class TestMsg: MsgBase
    {
        public TestMsg() { protoName = "TestMsg"; }
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public string str = "";
    }
}
