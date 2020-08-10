using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;

public class Echo : MonoBehaviour {
    Socket socket;
    public InputField InputField;
    public Text text;
    public GameObject textParent;

    byte[] readBuff = new byte[1024];
    string recvStr = "";

    public void Connection(){
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.BeginConnect("127.0.0.1", 8888,ConnectCallback,socket);
    }
    public void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
        }
    }
    public void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex){
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }
    public void Send()
    {
        string msg = InputField.text;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(msg);
        //socket.Send(sendBytes);
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);

        //byte[] readBuff = new byte[1024];
        //int count = socket.Receive(readBuff);
        //string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
        //text.text = recvStr;
        //socket.Close();
    }
    public void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);
            Debug.Log("Socket send succ" + count);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Send fail" + ex.ToString());
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(recvStr != "")
        {
            Text newText = Instantiate(this.text);
            newText.transform.parent = textParent.transform;
            newText.text = recvStr;
            Debug.Log("recvStr = " + recvStr);
            recvStr = "";
        }
	}
}
