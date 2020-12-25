using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace Assets.Script.proto
//{
    public class TestMsg : MsgBase
    {
        public TestMsg() { protoName = "TestMsg"; }
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public string str = "";
}
//}
