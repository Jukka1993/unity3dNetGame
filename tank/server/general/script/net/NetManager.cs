using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using System.Reflection;
using general.script.logic;
using general.script.proto;

namespace general.script.net
{
    public class NetManager
    {
        public static float aa = 0;
        public static float bb = 10;
        //监听socket
        public static Socket listenfd;
        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        //select的检查列表
        static List<Socket> checkRead = new List<Socket>();
        private static float msgSeq = 0;
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
                Socket.Select(checkRead, null, null, 5000);
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
                Close(cs, false);
                return;
            }
            try
            {
                count = clientfd.Receive(readbuff.bytes, readbuff.writeIdx, readbuff.remain, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Receive SocketException " + ex.ToString());
                Close(cs, false);
                return;
            }
            //客户端关闭
            if(count <= 0)
            {
                Console.WriteLine("Socket Close " + clientfd.RemoteEndPoint.ToString());
                Close(cs,true);
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
            //Console.WriteLine(aa++);
            //if (aa++ % 200 == 0)
            //{
            //    foreach (ClientState s in clients.Values)
            //    {
            //        //checkRead.Add(s.socket);
            //        TestMsg msg = new TestMsg();
            //        msg.str = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz";

            //        Send(s, msg);
            //    }
            //}
        }
        public static void OnReceiveData(ClientState cs)
        {
            ByteArray readBuff = cs.readBuff;
            //消息长度小于等于2,即还没消息长度都不能收完或者刚好收完，完全没有消息体,则return掉,等下次收到更多的字节了再来处理
            if(readBuff.length <= 2)
            {
                return;
            }
            //{
            //原来勘误上面有这里的修改,我还查了好久的问题才查出来
                byte[] bytes = readBuff.bytes;
                Int16 bodyLength = (Int16)((bytes[readBuff.readIdx + 1] << 8) | bytes[readBuff.readIdx]);
                if(readBuff.length < bodyLength + 2)
                {
                    return;
                }
                readBuff.readIdx += 2;
            //}

            //Int16 bodyLength = readBuff.ReadInt16();
            ////消息体长度比读缓冲区的内容长度还长，说明消息没收完，则return掉，等下次收到了更多的字节了再来处理
            //if(readBuff.length < bodyLength)
            //{
            //    Console.WriteLine("见此日志 readIndex 必连续加2，DecodeName极大概率出错");
            //    //此处有个坑爹的地方,如果readBuff.length小于bodyLength则会返回，下次再来获取数据
            //    //但是上面的bodyLength是通过ReadInt16得来的,即使这次获取数据不完整返回，也会导致
            //    //readIndex += 2; 所以一旦出现这种情况,就会导致 readIndex多向后偏移了两位,
            //    //从而在下面的DecodeName中，解析到的长度变得超级长（几万很常见）
            //    //如果此处要用 ReadInt16的话需要在下面的return之前将readIndex -= 2;
            //    //当然最好不要这样做，感觉这里之前就有坑,
            //    //见下面的AAA注释，而是将ReadInt16和readIndex+=2的操作分开比较好理解
            //    return;
            //}
            //开始解析协议名
            int nameCount = 0;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
            //解析协议名出错了,直接返回
            if(protoName == "")
            {
                Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
                Close(cs, false);
                return;
            }
            //协议名解析完了，读缓冲区的“开始读取索引”向后移动协议名的字节长度，用于之后直接开始解析协议体
            //Console.WriteLine("D nameCount += " + nameCount);
            readBuff.readIdx += nameCount;
            //AAA 开始解析协议体，这里为什么是bodyLength还要再看看才能想清楚。。。
            int bodyCount = bodyLength - nameCount;

                string name123 = System.Text.Encoding.UTF8.GetString(readBuff.bytes, 0, readBuff.bytes.Length);

            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            //协议体解析完了，将readIdx移动，便于之后再次解析下一条消息。
            //Console.WriteLine("D bodyCount += " + bodyCount);

            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            //分发消息，反射获取协议名对应的MethodInfo
            MethodInfo mi = typeof(MsgHandler).GetMethod("OnReceive" + protoName);
            object[] o = { cs, msgBase };
            //Console.WriteLine("Receive proto " + protoName);
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
        public static void Close(ClientState cs,bool isNormalClose)
        {
            //事件分发
            MethodInfo mei = typeof(general.script.logic.EventHandler).GetMethod("OnDisconnect");
            object[] ob = { cs,isNormalClose };
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
            //if (!cs.socket.Connected) //貌似这玩意只是说 上一次 有没有连上，还是怎么个意思，反正就是不可靠的意思
            //{
            //    return;
            //}
            msg.msgSeq = ++msgSeq;
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
                //Console.WriteLine("Send + " + cs.socket.RemoteEndPoint);
                cs.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Close on BeginSend " + ex.ToString());
            }
        }
        public static long GetTimeStamp()
        {
            long temp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            return temp;
            //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //return Convert.ToInt64(ts.TotalSeconds);
        }
    }
}
