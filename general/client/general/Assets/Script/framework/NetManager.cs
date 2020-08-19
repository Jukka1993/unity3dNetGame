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
    static byte[] readBuff;
    //写入队列
    static Queue<byte[]> writeQueue;
    static bool isConnecting = false;
    static bool isClosing = false;
    public delegate void EventListener(String err);
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
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
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readBuff = new byte[1024];
        writeQueue = new Queue<byte[]>();
        isConnecting = false;
        isClosing = false;
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
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
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
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
        //ByteArray
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(sendBytes);
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
        byte[] ba;//todo 看下完整的接收数据那一章,有提供一个类 ArrayBuffer
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        //ba.readIdx += count;
    }
}
