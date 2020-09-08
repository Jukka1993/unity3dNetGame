using System;
using System.Collections.Generic;
using System.Text;
using general.script.net;
using general.script.proto;
using general.script.db;
using general.script.logic;

namespace general.script.logic
{
    public partial class MsgHandler
    {
        public static void MsgRegister(ClientState cs, MsgBase msgBase)
        {
            MsgRegister msg = (MsgRegister)msgBase;
            //注册
            //int id = ;
            if (DBManager.Register(msg.name, msg.pw))
            {
                int id = DBManager.CheckPassword(msg.name, msg.pw);
                DBManager.CreatePlayer(id);
                msg.id = id;
                msg.result = 0;
            } else
            {
                msg.result = 1;
            }
            NetManager.Send(cs, msg);
        }
        public static void MsgLogout(ClientState cs,MsgBase msgBase)
        {
            MsgLogout msg = (MsgLogout)msgBase;
            if(cs.player == null)
        {
            return;
        }
            DBManager.UpdatePlayerData(cs.player.id, cs.player.data);
            PlayerManager.RemovePlayer(cs.player.id);
            cs.player = null;
            msg.result = 0;
            NetManager.Send(cs,msg);
        }
        public static void MsgLogin(ClientState cs,MsgBase msgBase)
        {
            MsgLogin msg = (MsgLogin)msgBase;
            int id = DBManager.CheckPassword(msg.name, msg.pw);
            if (id < 0)
            {
                msg.result = 1;
                NetManager.Send(cs, msg);
                return;
            }
            //不允许再次登录(这条socket连接已经有一个player了
            if(cs.player != null)
            {
                msg.result = 1;
                NetManager.Send(cs, msg);
                return;
            }
            //如果该账号已被其他人登录，则踢出其他人
            if (PlayerManager.IsOnline(id))
            {
                Player other = PlayerManager.GetPlayer(id);
                MsgKick msgKick = new MsgKick();
                msgKick.reason = 0;
            DBManager.UpdatePlayerData(other.id, other.data);
            PlayerManager.RemovePlayer(other.id);
                other.Send(msgKick);
            other.cs.player = null;
                //NetManager.Close(other.cs);
            }
            //获取玩家数据
            PlayerData playerData = DBManager.GetPlayerData(id);
            if(playerData == null)
            {
                msg.result = 1;
                NetManager.Send(cs, msg);
                return;
            }
            Player player = new Player(cs);
            player.id = id;
            player.data = playerData;
            PlayerManager.AddPlayer(id, player);
            cs.player = player;
            //返回协议
            msg.result = 0;
            player.Send(msg);
        }
    }
}
