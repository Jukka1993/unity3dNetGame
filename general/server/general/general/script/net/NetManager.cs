using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using System.Reflection;
//using general.script.logic;

namespace general.script.net
{
    public class NetManager
    {
        //监听socket
        public static Socket listenfd;
        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        //select的检查列表
        static List<Socket> checkRead = new List<Socket>();
        //ping间隔
        public static long pingInterval = 30;
        public static void StartLoop(int listenPort)
        {
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("0.0.0.0");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, listenPort);
            listenfd.Bind(ipEp);
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            while (true)
            {
                ResetCheckRead();
                Socket.Select(checkRead, null, null, 1000);
                for(int i = checkRead.Count - 1; i >= 0; i--)
                {
                    Socket s = checkRead[i];
                    if(s == listenfd)
                    {
                        ReadListenfd(s);
                    } else
                    {
                        ReadClientfd(s);
                    }
                }
                Timer();
            }

        }
        public static void ResetCheckRead()
        {
            checkRead.Clear();
            checkRead.Add(listenfd);
            foreach(ClientState s in clients.Values)
            {
                checkRead.Add(s.socket);
            }
        }
        public static void ReadListenfd(Socket listenfd)
        {
            try
            {
                Socket clientfd = listenfd.Accept();
                Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
                ClientState cs = new ClientState();
                cs.socket = clientfd;
                cs.lastPingTime = NetManager.GetTimeStamp();
                clients.Add(clientfd, cs);
            } catch(SocketException ex)
            {
                Console.WriteLine("Accept fail " + ex.ToString());
            }
        }
        public static void ReadClientfd(Socket clientfd)
        {
            ClientState cs = clients[clientfd];
            ByteArray readbuff = cs.readBuff;
            int count = 0;
            //缓冲区不够,清除
            if(readbuff.remain <= 0)
            {
                OnReceiveData(cs);
                readbuff.MoveBytes();
            }
            //清除之后，依然不够，只能返回
            //缓冲区长度只有1024,单条协议超过缓冲区长度时会发生错误,根据需要调整长度
            if(readbuff.remain <= 0)
            {
                Console.WriteLine("Receive fail, maybe msg length > buff capacity");
                Close(cs);
                return;
            }
            try
            {
                count = clientfd.Receive(readbuff.bytes, readbuff.writeIdx, readbuff.remain, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Receive SocketException " + ex.ToString());
                Close(cs);
                return;
            }
            //客户端关闭
            if(count <= 0)
            {
                Console.WriteLine("Socket Close " + clientfd.RemoteEndPoint.ToString());
                Close(cs);
                return;
            }
            //消息处理
            readbuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData(cs);
            //移动缓冲区
            readbuff.CheckAndMoveBytes();
        }
        public static void Timer()
        {
            MethodInfo mei = typeof(general.script.logic.EventHandler).GetMethod("OnTimer");
            object[] ob = { };
            mei.Invoke(null, ob);
        }
        public static void OnReceiveData(ClientState cs)
        {
            ByteArray readBuff = cs.readBuff;
            //消息长度小于等于2,即还没消息长度都不能收完或者刚好收完，完全没有消息体,则return掉,等下次收到更多的字节了再来处理
            if(readBuff.length <= 2)
            {
                return;
            }
            Int16 bodyLength = readBuff.ReadInt16();
            //消息体长度比读缓冲区的内容长度还长，说明消息没收完，则return掉，等下次收到了更多的字节了再来处理
            if(readBuff.length < bodyLength)
            {
                return;
            }
            //开始解析协议名
            int nameCount = 0;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
            //解析协议名出错了,直接返回
            if(protoName == "")
            {
                Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
                Close(cs);
            }
            //协议名解析完了，读缓冲区的“开始读取索引”向后移动协议名的字节长度，用于之后直接开始解析协议体
            readBuff.readIdx += nameCount;
            //开始解析协议体，这里为什么是bodyLength还要再看看才能想清楚。。。
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            //协议体解析完了，将readIdx移动，便于之后再次解析下一条消息。
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            //分发消息，反射获取协议名对应的MethodInfo
            MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
            object[] o = { cs, msgBase };
            Console.WriteLine("Receive proto " + protoName);
            if(mi != null)
            {
                mi.Invoke(null, o);
            } else
            {
                Console.WriteLine("OnReceiveData Invoke fail " + protoName);

            }
            //若本次消息处理完,缓冲区剩余未读取字节数大于2个字节，说明可能存在完整的下一条消息，则直接开始尝试处理下一条消息（若没有则会自己返回的）
            if(readBuff.length > 2)
            {
                OnReceiveData(cs);
            }

        }
        public static void Close(ClientState cs)
        {
            //事件分发
            MethodInfo mei = typeof(general.script.logic.EventHandler).GetMethod("OnDisconnect");
            object[] ob = { cs };
            mei.Invoke(null, ob);
            //关闭
            cs.socket.Close();
            clients.Remove(cs.socket);
        }
        public static void Send(ClientState cs, MsgBase msg)
        {
            //状态判断
            //cs为null则return
            if(cs == null)
            {
                return;
            }
            //cs的socket未连接则return
            if (!cs.socket.Connected)
            {
                return;
            }
            //数据编码
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] sendBytes = new byte[2 + len];
            //组装消息长度
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);
            //组装消息名
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            //组装消息体
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
            try
            {
                cs.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Close on BeginSend " + ex.ToString());
            }
        }
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }
}
