﻿using System;
using System.Collections.Generic;
using System.Text;


    public class MsgRegister:MsgBase
    {
        public MsgRegister()
        {
            protoName = "MsgRegister";
        }
        //客户端发
        public string name = "";
        public string pw = "";
        //服务器回,
        public int id = -1;
        public int result = 0;
    }
    public class MsgLogin : MsgBase
    {
        public MsgLogin()
        {
            protoName = "MsgLogin";
        }
        public string name = "";
        public string pw = "";
        public int result = 0;
    }
    public class MsgKick : MsgBase
    {
        public MsgKick()
        {
            protoName = "MsgKick";
        }
        public int reason = 0;
    }

