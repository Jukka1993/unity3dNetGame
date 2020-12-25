using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Linq;

public enum NetEvent
{
    ConnectSucc = 1,
    ConnectFail = 2,
    Close = 3,
}


public static class NetManager {
    //定义套接字
    static Socket socket;
    //接收缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;
    static bool isConnecting = false;
    static bool isClosing = false;
    static List<MsgBase> msgList = new List<MsgBase>();
    static int msgCount = 0;
    readonly static int MAX_MESSAGE_FIRE = 10;
    public static bool isUsePing = true;
    public static int pingInterval = 30000;
    static float lastPingTime = 0;
    static float lastPongTime = 0;

    public delegate void EventListener(String err);
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    public delegate void UpdateText(string tt);
    public static UpdateText updateText = null;
    public static UpdateText updateText2 = null;
    public static UpdateText updateText3 = null;
    public static UpdateText updateText4 = null;

    public delegate void MsgListener(MsgBase msgBase);
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
    private static int index = 0;
    private static int index2 = 0;
    private static int index3 = 0;
    private static int index4 = 0;
    private static float msgSeq = 0;

    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        } else
        {
            eventListeners[netEvent] = listener;
        }
    }
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }
    private static void FireEvent(NetEvent netEvent, String err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }
    public static void AddMsgListener(string msgName,MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        } else
        {
            msgListeners[msgName] = listener;
        }
    }
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
            if(msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }
    }
    private static void FireMsg(string msgName,MsgBase msgBase)
    {
        //Debug.Log("FireMsg ====== "+ msgName);
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }
    public static void Connect(string ip,int port)
    {
        if(socket != null && socket.Connected)
        {
            Debug.Log("Connect fail, already connected!");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("Connect fail, isConnecting!");
            return;
        }
        InitState();
        socket.NoDelay = true;
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }
    private static void InitState()
    {
        lastPingTime = Time.time;
        lastPongTime = Time.time;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readBuff = new ByteArray();
        writeQueue = new Queue<ByteArray>();
        msgList = new List<MsgBase>();
        msgCount = 0;
        isConnecting = false;
        isClosing = false;
        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }
    private static void OnMsgPong(MsgBase msgBase)
    {
        Debug.Log("Receive MsgPond");
        lastPongTime = Time.time;
    }
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            if(count == 0)
            {
                Close();
                return;
            }
            readBuff.writeIdx += count;
            updateText(index++ + "count = " + count + "\n" + "receiveCallBack readBuff.writeIdx = " + readBuff.writeIdx);
            //Debug.Log("receiveCallBack readBuff.writeIdx = " + readBuff.writeIdx);
            OnReceiveData();
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }catch(SocketException ex)
        {
            Debug.Log("Socket Receive fail " + ex.ToString());
        }
    }
    private static void OnReceiveData()
    {
        if(readBuff.length <= 2)
        {
            return;
        }
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if(readBuff.length < bodyLength + 2)
        {
            return;
        }
        readBuff.readIdx += 2;
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        //Debug.Log("protoName = " + protoName);
        if (protoName == "")
        {
            Debug.Log("OnReceiveData MsgBase.DecodeName fail");
            return;
        }
        //Debug.Log("A protoName = " + protoName);

        readBuff.readIdx += nameCount;
        //Debug.Log("B protoName = " + protoName);

        int bodyCount = bodyLength - nameCount;
        //Debug.Log("C protoName = " + protoName);

        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        //Debug.Log("D protoName = " + protoName);
        updateText2(index2++ + " 收到协议 " + protoName);

        readBuff.readIdx += bodyCount;
        //Debug.Log("E protoName = " + protoName);

        readBuff.CheckAndMoveBytes();
        //Debug.Log("11 protoName = " + protoName);

        lock (msgList)
        {
            //if(protoName == "MsgGetAchieve")
            //{
            //    Debug.Log("11 protoName = " + protoName + " " + msgBase);
            //}
            updateText3(index3++ + " 添加到msgList " + msgBase.protoName);
            msgList.Add(msgBase);
            //if (protoName == "MsgGetAchieve")
            //{
            //    Debug.Log("22 protoName = " + protoName + " " + msgBase);
            //}
            


        }
        msgCount++;
        //Debug.Log("msgCount = " + msgCount);
        if (readBuff.length > 2)
        {
            OnReceiveData();
        }
    }
    public static void Close()
    {
        if(socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        } else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }
    public static void Send(MsgBase msg)
    {
        if(socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        if (isClosing)
        {
            return;
        }
        msg.msgSeq = ++msgSeq;
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
        //ByteArray
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        if(count == 1)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }
    }
    public static void SendCallback(IAsyncResult ar)
    {
        Socket socket =(Socket) ar.AsyncState;
        if(socket == null || !socket.Connected)
        {
            return;
        }
        int count = socket.EndSend(ar);
        ByteArray ba;//todo 看下完整的接收数据那一章,有提供一个类 ArrayBuffer
        lock (writeQueue)
        {
            ba = writeQueue.FirstOrDefault();
        }
        if(ba != null)
        {
            ba.readIdx += count;
            if (ba.length == 0)
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    //ba = writeQueue.First();
                    ba = writeQueue.FirstOrDefault();
                }
            }
        }
        if(ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        } else if (isClosing)
        {
            socket.Close();
        }
    }
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }
    public static void PingUpdate()
    {
        if (!isUsePing)
        {
            return;
        }
        if(Time.time -lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Debug.Log("Send(msgPing);");
            Send(msgPing);
            lastPingTime = Time.time;
        }
        if(Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }
    public static void MsgUpdate()
    {
        if (msgCount == 0)
        {
            return;
        }
        for(int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            MsgBase msgBase = null;
            lock (msgList)
            {
                if(msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    updateText4(index4++ + "取出协议" + msgBase.protoName);
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            if(msgBase != null)
            {
                FireMsg(msgBase.protoName, msgBase);
            } else
            {
                break;
            }
        }
    }
}
