using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Reflection;

namespace LuandouServer
{
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
        public int hp = 100;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float eulY = 0;
    }
    class Program
    {
        static Socket listenfd;
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        static void Main(string[] args)
        {
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddres = IPAddress.Parse("127.0.0.1");
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddres, 8888);
            listenfd.Bind(iPEndPoint);
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            List<Socket> checkRead = new List<Socket>();
            while (true)
            {
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach(ClientState s in clients.Values)
                {
                    checkRead.Add(s.socket);
                }
                Socket.Select(checkRead, null, null, 1000);
                foreach(Socket s in checkRead)
                {
                    if(s == listenfd)
                    {
                        ReadListenfd(s);
                    } else
                    {
                        ReadClientfd(s);
                    }
                }
            }
        }
        static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }
        static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }catch(SocketException ex)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Receive SocketException " + ex.ToString());
                return false;
            }
            //客户端关闭
            if(count == 0)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }
            string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            string[] split = recvStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
            object[] o = { state, msgArgs };
            mi.Invoke(null, o);
            
            Console.WriteLine("Receive " + recvStr);

            //string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
            //string sendStr = recvStr;
            //byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            //foreach(ClientState cs in clients.Values)
            //{
            //    cs.socket.Send(sendBytes);
            //}
            return true;
        }
    }
}
