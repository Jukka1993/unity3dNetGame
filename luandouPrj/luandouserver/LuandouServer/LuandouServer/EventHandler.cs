using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuandouServer
{
    class EventHandler
    {
        public static void OnDisconnect(ClientState c)
        {
            Console.WriteLine("OnDisconnect");
            foreach(ClientState cs in Program.clients.Values)
            {
                if (cs.socket != c.socket)
                {
                    //cs.socket.Send()
                    string sendStr = "Leave|";
                    sendStr += c.socket.RemoteEndPoint.ToString();
                    byte[] sendByte = System.Text.Encoding.Default.GetBytes(sendStr);
                    cs.socket.Send(sendByte);
                }
            }
        }
    }
}
