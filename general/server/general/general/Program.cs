﻿using System;
using general.script.net;
using general.script.db;

namespace general
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //MsgMove msgMove = new MsgMove();
            //msgMove.x = 10;
            //msgMove.y = 11;
            //msgMove.z = 12;
            //byte[] bytes = MsgBase.Encode(msgMove);
            //string s = System.Text.Encoding.UTF8.GetString(bytes);
            //Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            //Console.WriteLine(s);
            //Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            //MsgMove msgMove1 = (MsgMove)MsgBase.Decode("MsgMove", bytes,0,bytes.Length);
            //Console.WriteLine(msgMove1.protoName);
            //Console.WriteLine(msgMove1.x);
            //Console.WriteLine(msgMove1.y);
            //Console.WriteLine(msgMove1.z);
            //Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            if (!DBManager.Connect("netgame", "127.0.0.1", 3306, "root", ""))
            {
                return;
            }
            bool is1Exist = DBManager.IsAccountExist("1");
            bool is2Exist = DBManager.IsAccountExist("2");
            bool is3Exist = DBManager.IsAccountExist("3");
            bool is4Exist = DBManager.IsAccountExist("4");
            Console.WriteLine("is1Exist " + is1Exist);
            Console.WriteLine("is2Exist " + is2Exist);
            Console.WriteLine("is3Exist " + is3Exist);
            Console.WriteLine("is4Exist " + is4Exist);
            NetManager.StartLoop(8888);
            




            Console.ReadKey();
        }
    }
}
