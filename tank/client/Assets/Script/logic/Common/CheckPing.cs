using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CheckPing
{
    public static CheckPing _instance = null;
    public float lastCheckPingTime = 0;
    public double clientSendTime1 = 0;
    public double serverReceiveTime2 = 0;
    public double serverSendTime3 = 0;
    public double clientReceiveTime4 = 0;
    public double clientToServerPing = 0;
    public double serverDealTime = 0;
    public double serverToClientPing = 0;
    public delegate void AfterCheckPing(double pingTime);
    public AfterCheckPing afterCheckPing = null;
    public static CheckPing Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CheckPing();
                NetManager.AddMsgListener("PingCheckMsg", _instance.onPingCheckMsg);
            }
            return _instance;
        }
        
    }
    public void checkPing()
    {
        if (Time.time > lastCheckPingTime + 1)
        {
            PingCheckMsg msg = new PingCheckMsg();
            msg.clientSendTime1 = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            NetManager.Send(msg);
            lastCheckPingTime = Time.time;
        }
    }
    public void onPingCheckMsg(MsgBase baseMsg)
    {
        PingCheckMsg msg = (PingCheckMsg)baseMsg;
        clientSendTime1 = msg.clientSendTime1;
        serverReceiveTime2 = msg.serverReceiveTime2;
        serverSendTime3 = msg.serverSendTime3;
        clientReceiveTime4 = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;

        clientToServerPing = serverReceiveTime2 - clientSendTime1;
        serverDealTime = serverSendTime3 - serverReceiveTime2;
        serverToClientPing = clientReceiveTime4 - serverSendTime3;
        if (afterCheckPing != null)
        {
            afterCheckPing((clientReceiveTime4 - clientSendTime1)/2);
        }
    }
}

