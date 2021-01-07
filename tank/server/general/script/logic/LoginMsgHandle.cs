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
                CommonUtil.Log("收到 " + cs.socket.RemoteEndPoint.ToString() + " 的注册请求,执行注册操作, 申请到的 id 为 " + id);
            }
            else
            {
                msg.reasonStr = reasonStr;
                msg.result = 1;
                CommonUtil.Log("收到 " + cs.socket.RemoteEndPoint.ToString() + " 的注册请求,拒绝原因为： " + reasonStr);
            }
            NetManager.Send(cs, msg);
        }
        public static void OnReceiveMsgLogout(ClientState cs, MsgBase msgBase)
        {
            MsgLogout msg = (MsgLogout)msgBase;
            if (cs.player == null)
            {
                CommonUtil.Log("收到 " + cs.socket.RemoteEndPoint.ToString() + " 的退出登出请求,但它本身就没登出");
                return;
            }
            CommonUtil.Log("收到 " + cs.socket.RemoteEndPoint.ToString() + " 的退出登录请求,执行登出操作");
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
                CommonUtil.Log("收到 " + cs.socket.RemoteEndPoint.ToString() + "的登录请求,拒绝原因:" + reasonStr);

                return;
            }
            //不允许再次登录(这条socket连接已经有一个player了
            if (cs.player != null)
            {
                msg.result = 1;
                msg.reasonStr = reasonStr;
                NetManager.Send(cs, msg);
                CommonUtil.Log("收到 " + cs.socket.RemoteEndPoint.ToString() + "的登录请求,但它已经登录过了，因此拒绝");

                return;
            }
            Player player = null;
            //如果该账号在playermanager中,且存在一个连接,断开之前的连接,相当于要踢走之前登陆的那个客户端
            //如果该账号在playermanager中,但没有连接,则说明可能是断线了的角色(仍然存在于房间中),则不需要踢走(没得踢的),而是用现在这个新的连接,连上这个账号角色
            if (PlayerManager.ExistPlayer(id))
            {
                if (PlayerManager.GetPlayer(id).cs != null)
                {
                    Player other = PlayerManager.GetPlayer(id);
                    player = other;
                    CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 登录时发现 PlayerManager 中存在已有连接 " + other.cs.socket.RemoteEndPoint.ToString() + " 的player " + id + " 即将把这个player断开已有cs并连接到新cs");
                    MsgKick msgKick = new MsgKick();
                    msgKick.reason = 0;
                    msgKick.reasonStr = "other login with your account";
                    DBManager.UpdatePlayerData(other.id, other.data);
                    other.Send(msgKick);
                    other.UnBindCS();
                }
                else
                {
                    CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 登录时发现 PlayerManager 中存在无 cs 的player " + id + " 即将把这个player绑定到该连接");
                    player = PlayerManager.GetPlayer(id);
                }
            }
            else
            {
                CommonUtil.Log(cs.socket.RemoteEndPoint.ToString() + " 登录时发现 PlayerManager 中不存在玩家 " + id + " 即将创建一个新 player");
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
                CommonUtil.Log("创建了一个新 player 并发起登录请求的 " + cs.socket.RemoteEndPoint.ToString() + " 绑定到这个新player上");
                player = new Player(cs);
            }
            else
            {
                CommonUtil.Log("将发起登录请求的 " + cs.socket.RemoteEndPoint.ToString() + " 绑定到已有的 player 上");
                player.BindCS(cs);
            }
            player.name = playerName;
            player.id = id;
            CommonUtil.Log("将player的id设为 " + id);
            player.data = playerData;
            if (!PlayerManager.ExistPlayer(id))
            {
                CommonUtil.Log("PlayerManager 增加玩家 " + id);
                PlayerManager.AddPlayer(id, player);
            }
            cs.player = player;
            //返回协议
            msg.result = 0;
            msg.id = id;
            msg.roomId = player.roomId;
            player.Send(msg);
            CommonUtil.Log("登录了一个用户");
            if (msg.roomId >= 0)
            {
                CommonUtil.Log("用户应该在房间内");
                Room room = RoomManager.GetRoom(player.roomId);
                if (room != null)
                {
                    CommonUtil.Log("服务器尝试将用户拉到房间内");
                    room.ReEnterRoom(player);
                }
            }
        }
    }
}
