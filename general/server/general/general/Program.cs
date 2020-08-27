using System;

namespace general
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MsgMove msgMove = new MsgMove();
            msgMove.x = 10;
            msgMove.y = 11;
            msgMove.z = 12;
            byte[] bytes = MsgBase.Encode(msgMove);
            string s = System.Text.Encoding.UTF8.GetString(bytes);
            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            Console.WriteLine(s);
            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            MsgMove msgMove1 = (MsgMove)MsgBase.Decode("MsgMove", bytes,0,bytes.Length);
            Console.WriteLine(msgMove1.protoName);
            Console.WriteLine(msgMove1.x);
            Console.WriteLine(msgMove1.y);
            Console.WriteLine(msgMove1.z);
            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");




            Console.ReadKey();
        }
    }
}
