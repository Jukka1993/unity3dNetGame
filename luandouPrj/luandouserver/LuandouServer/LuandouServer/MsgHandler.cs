using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuandouServer
{
    class MsgHandler
    {
        public static void MsgEnter(ClientState c,string msgArgs)
        {
            Console.WriteLine("MsgEnter" + msgArgs);
            //string[] split = msgArgs.Split(',');
            //string desc = split[0];
            //float x = float.Parse(split[1]);
            //float y = float.Parse(split[2]);
            //float z = float.Parse(split[3]);
            //float eulY = float.Parse(split[4]);
            //c.hp = 100;
            //c.x = x;
            //c.y = y;
            //c.z = z;
            //c.eulY = eulY;
            string sendStr = "Enter|" + msgArgs;
            byte[] sendByte = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach(ClientState cs in Program.clients.Values)
            {
                if(cs.socket != c.socket)
                {
                    cs.socket.Send(sendByte);
                }
            }

        }
        public static void MsgList(ClientState c, string msgArgs)
        {
            Console.WriteLine("MsgList" + msgArgs);
            string sendStr = "List|";
            foreach(ClientState cs in Program.clients.Values)
            {
                sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
                sendStr += cs.x.ToString() + ",";
                sendStr += cs.y.ToString() + ",";
                sendStr += cs.z.ToString() + ",";
                sendStr += cs.eulY.ToString() + ",";
                sendStr += cs.hp.ToString() + ",";
            }
            //byte[] sendBytes  = new byte
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            c.socket.Send(sendBytes);
        }
        public static void MsgMove(ClientState c, string msgArgs)
        {
            Console.WriteLine("MsgMove" + msgArgs);
            string sendStr = "Move|";
            sendStr += msgArgs;
            string[] split = msgArgs.Split(',');
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            //float eulY = float.Parse(split[4]);
            //int hp = int.Parse(split[5]);
            c.x = x;
            c.y = y;
            c.z = z;
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            foreach(ClientState cs in Program.clients.Values)
            {
                if(cs.socket != c.socket)
                {
                    cs.socket.Send(sendBytes);
                }
            }
        }

    }
}
