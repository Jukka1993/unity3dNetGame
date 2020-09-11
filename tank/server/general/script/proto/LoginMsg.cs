using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;

namespace general.script.proto
{
    public class MsgRegister : MsgBase
    {
        public MsgRegister()
        {
            protoName = "MsgRegister";
        }
        //客户端发
        public string name = "";
        public string pw = "";
        //服务器回,
        public string reasonStr = "";
        public int id = -1;
        public int result = 0;
    }
    public class MsgLogin : MsgBase
    {
        public MsgLogin()
        {
            protoName = "MsgLogin";
        }
        public string reasonStr = "";
        public string name = "";
        public string pw = "";
        public int id = -1;
        public int result = 0;
    }
    public class MsgLogout : MsgBase
    {
        public MsgLogout()
        {
            protoName = "MsgLogout";
        }
        public int result = 0;
    }
    public class MsgKick : MsgBase
    {
        public MsgKick()
        {
            protoName = "MsgKick";
        }
        public string reasonStr = "";
        public int reason = 0;
    }
}
