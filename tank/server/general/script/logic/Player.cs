using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;

namespace general.script.logic
{
    public enum PlayerState
    {
        OutRoom,
        Preparing,
        Fighting
    }
    public class Player
    {
        public bool Connected
        {
            get
            {
                return cs == null;
            }
        }
        public PlayerState status = PlayerState.OutRoom;
        public int id = -1;
        public string name = "";
        public ClientState cs;
        public float x;
        public float y;
        public float z;
        public float ex;
        public float ey;
        public float ez;
        public float turretY;
        public int roomId = -1;
        public int camp = 1;
        public int hp = 100;
        public long lastPingTime = 0;
        public PlayerData data;
        public Player(ClientState state)
        {
            CommonUtil.Log("new 一个 player,并绑定到 " + state.socket.RemoteEndPoint.ToString());
            cs = state;
            status = PlayerState.OutRoom;
            lastPingTime = cs.lastPingTime;
        }
        public void Send(MsgBase msgBase)
        {
            if (cs == null)
            {
                return;
            }
            
            if (!CommonUtil.IsFilterProto(msgBase.protoName))
            {
                CommonUtil.Log("向 " + cs.socket.ToString() + " 发送 " + MsgBase.ToString(msgBase));
            }
            NetManager.Send(cs, msgBase);
        }
        public void BreakFromCS(bool breakConnect = true, bool isNormalBreak = true)
        {
            if (cs == null)
            {
                return;
            }
            ClientState oldCs = this.cs;
            this.cs = null;
            if (breakConnect)
            {
                if (isNormalBreak)
                {
                    CommonUtil.Log("将player", id.ToString(), "与", cs.socket.RemoteEndPoint.ToString(), "解绑,并正常关闭 cs");
                }
                else
                {
                    CommonUtil.Log("将player", id.ToString(), "与", cs.socket.RemoteEndPoint.ToString(), "解绑,并异常关闭 cs");
                }
                NetManager.Close(oldCs, isNormalBreak);
            }
            else
            {
                CommonUtil.Log("将player", id.ToString(), "与",cs.socket.RemoteEndPoint.ToString(),"解绑,但不关闭 cs");
            }
        }
        public void UnBindCS()
        {
            ClientState oldCs = cs;
            cs = null;
            if (oldCs != null)
            {
                CommonUtil.Log("将player", id.ToString(), "与", oldCs.socket.RemoteEndPoint.ToString(), "解绑,并不正常关闭 cs");
                NetManager.Close(oldCs, false);
            }
            NotifyRoom();
        }
        public void BindCS(ClientState newCs)
        {
            CommonUtil.Log("将player", id.ToString(), "与", newCs.socket.RemoteEndPoint.ToString(), "绑定");
            cs = newCs;
            lastPingTime = cs.lastPingTime;
            NotifyRoom();
        }
        public void NotifyRoom()
        {
            if (roomId >= 0)
            {
                Room room = RoomManager.GetRoom(roomId);
                if (room != null)
                {
                    room.PlayerStatusChange();
                }
            }
        }
    }
}
