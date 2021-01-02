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
        public static void OnReceiveMsgRegister(ClientState cs, MsgBase msgBase)
        {
            MsgRegister msg = (MsgRegister)msgBase;
            //注册
            //int id = ;
            string reasonStr = "";
            if (DBManager.Register(msg.name, msg.pw, out reasonStr))
            {
                int id = DBManager.CheckPassword(msg.name, msg.pw);
                DBManager.CreatePlayer(id);
                msg.id = id;
                msg.result = 0;
            }
            else
            {
                msg.reasonStr = reasonStr;
                msg.result = 1;
            }
            NetManager.Send(cs, msg);
        }
        public static void OnReceiveMsgLogout(ClientState cs, MsgBase msgBase)
        {
            MsgLogout msg = (MsgLogout)msgBase;
            if (cs.player == null)
            {
                return;
            }
            DBManager.UpdatePlayerData(cs.player.id, cs.player.data);
            PlayerManager.RemovePlayer(cs.player.id);
            cs.player = null;
            msg.result = 0;
            NetManager.Send(cs, msg);
        }
        public static void OnReceiveMsgLogin(ClientState cs, MsgBase msgBase)
        {
            MsgLogin msg = (MsgLogin)msgBase;
            string reasonStr = "";
            int id = DBManager.CheckPassword(msg.name, msg.pw, out reasonStr);
            if (id < 0)
            {
                msg.result = 1;
                msg.reasonStr = reasonStr;
                msg.id = id;
                NetManager.Send(cs, msg);
                return;
            }
            //不允许再次登录(这条socket连接已经有一个player了
            if (cs.player != null)
            {
                msg.result = 1;
                NetManager.Send(cs, msg);
                msg.reasonStr = reasonStr;

                return;
            }
            Player player = null;
            //如果该账号在playermanager中,且存在一个连接,断开之前的连接,相当于要踢走之前登陆的那个客户端
            //如果该账号在playermanager中,但没有连接,则说明可能是断线了的角色(仍然存在于房间中),则不需要踢走(没得踢的),而是用现在这个新的连接,连上这个账号角色
            if (PlayerManager.ExistPlayer(id) && PlayerManager.GetPlayer(id).cs != null)
            {
                Player other = PlayerManager.GetPlayer(id);
                player = other;
                MsgKick msgKick = new MsgKick();
                msgKick.reason = 0;
                msgKick.reasonStr = "other login with your account";
                DBManager.UpdatePlayerData(other.id, other.data);
                //PlayerManager.RemovePlayer(other.id);
                other.Send(msgKick);
                //other.BreakFromCS(true, false);
                other.UnBindCS();
            }
            //获取玩家数据
            PlayerData playerData = DBManager.GetPlayerData(id);
            string playerName = DBManager.GetPlayerName(id);
            if (playerData == null)
            {
                if (player != null)
                {
                    PlayerManager.RemovePlayer(player.id);
                }
                msg.result = 1;
                msg.reasonStr = reasonStr;
                NetManager.Send(cs, msg);
                return;
            }
            if (player == null)
            {
                player = new Player(cs);
            }
            else
            {
                player.BindCS(cs);
            }
            player.name = playerName;
            player.id = id;
            player.data = playerData;
            if (!PlayerManager.ExistPlayer(id))
            {
                PlayerManager.AddPlayer(id, player);
            }
            cs.player = player;
            //返回协议
            msg.result = 0;
            msg.id = id;
            msg.roomId = player.roomId;
            player.Send(msg);
            Console.WriteLine("登录了一个用户");
            if (msg.roomId >= 0)
            {
                Console.WriteLine("用户应该在房间内");
                Room room = RoomManager.GetRoom(player.roomId);
                if (room != null)
                {
                    Console.WriteLine("服务器尝试将用户拉到房间内");
                    room.ReEnterRoom(player);
                }
            }
        }
    }
}
