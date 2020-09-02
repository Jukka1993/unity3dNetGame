using System;
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
            //if (DBManager.Register("jukka", "12345"))
            //{
            //    Console.WriteLine("注册成功");
            //}
            //if (DBManager.CreatePlayer(1))
            //{
            //    Console.WriteLine("创建成功");
            //}
            if (DBManager.CheckPassword("jukka", "123452"))
            {
                Console.WriteLine("用户名密码正确");
            } else
            {
                Console.WriteLine("用户名或密码错误");
            }
            //bool is1Exist = DBManager.IsNameExist("1");
            //bool is2Exist = DBManager.IsNameExist("2");
            //bool is3Exist = DBManager.IsNameExist("3");
            //bool is4Exist = DBManager.IsNameExist("4");
            //Console.WriteLine("is1Exist " + is1Exist);
            //Console.WriteLine("is2Exist " + is2Exist);
            //Console.WriteLine("is3Exist " + is3Exist);
            //Console.WriteLine("is4Exist " + is4Exist);
            NetManager.StartLoop(8888);
            




            Console.ReadKey();
        }
    }
}
