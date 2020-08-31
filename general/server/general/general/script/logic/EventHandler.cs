using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;

namespace general.script.logic
{
    public partial class EventHandler
    {
        public static void OnDisconnect(ClientState cs)
        {
            Console.WriteLine("Close");
        }
        public static void OnTimer()
        {
            CheckPing();
        }
        public static void CheckPing()
        {
            long timeNow = NetManager.GetTimeStamp();
            long overtime = NetManager.pingInterval * 4;
            foreach (ClientState cs in NetManager.clients.Values)
            {
                if(timeNow -cs.lastPingTime > overtime)
                {
                    Console.WriteLine("Ping Close " + cs.socket.RemoteEndPoint.ToString());
                    NetManager.Close(cs);
                    return;
                }
            }
        }
    }
}
