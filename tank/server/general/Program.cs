using System;
using general.script.net;
using general.script.db;
using general.script.logic;

namespace general
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!DBManager.Connect(Constant.dbName, Constant.dbIP, Constant.dbPort, Constant.dbLoginAccount, Constant.dbPassWord))
            {
                return;
            }
            NetManager.StartLoop(8888);
            
            Console.ReadKey();
        }
    }
}
