using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
namespace EchoPrjServer
{
    class Program
    {
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        static void Main(string[] args)
        {
            Console.WriteLine("HelloWorld");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            socket.Bind(ipEp);
            socket.Listen(0);
            Console.WriteLine("服务器启动成功...");
            socket.BeginAccept(AcceptCallback, socket);
            Console.ReadLine();
            
            //Socket connfd = socket.Accept();
            //Console.WriteLine("[服务器]Accept");
            //while (true)
            //{
            //    byte[] readBuff = new byte[1024];
            //    int count = connfd.Receive(readBuff);
            //    string readStr = System.Text.Encoding.Default.GetString(readBuff);
            //    Console.WriteLine("[服务器接收]" + readStr);
            //    byte[] sendBytes = System.Text.Encoding.Default.GetBytes(readStr);
            //    connfd.Send(sendBytes);
            //}
        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("[server] accept");
                Socket listenfd = (Socket)ar.AsyncState;
                Socket clientfd = listenfd.EndAccept(ar);

                ClientState state = new ClientState();
                state.socket = clientfd;
                clients.Add(clientfd, state);
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
                listenfd.BeginAccept(AcceptCallback, listenfd);
                //clients.Keys.GetEnumerator().MoveNext();
                //var cur = clients.Keys.GetEnumerator().Current;
                //Console.WriteLine("cur1");
                //Console.WriteLine(cur);
                //Console.WriteLine("cur2");
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Accept fail" + ex.ToString());
            }
        }
        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ClientState state = (ClientState)ar.AsyncState;
                Socket clientfd = state.socket;
                int count = clientfd.EndReceive(ar);
                if (count == 0)
                {
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("Socket Close");
                    return;
                }
                string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
                byte[] sendBytes =
                    System.Text.Encoding.Default.GetBytes(recvStr);
                foreach(KeyValuePair<Socket, ClientState> kvp in clients)
                {
                    kvp.Key.Send(sendBytes);
                }
                //clientfd.Send(sendBytes);
                //ClientState clientCur = new ClientState();
                //clients.Keys.GetEnumerator().MoveNext();
                //var cur = clients.Keys.GetEnumerator().Current;
                //var cur = clients.Keys.GetEnumerator().Current;
                //Console.WriteLine("cur11");
                //Console.WriteLine(cur.ToString());
                //Console.WriteLine("cur22");
                //clients.TryGetValue(cur, out clientCur);
                //clientCur.socket.Send(sendBytes);
                //while (clients.Keys.GetEnumerator().MoveNext())
                //{
                //    clients.TryGetValue(clients.Keys.GetEnumerator().Current, out clientCur);
                //    clientCur.socket.Send(sendBytes);
                //}
                Console.WriteLine("receve data");
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("socket Receive fail" + ex.ToString());
            }

        }
    }
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }
}
