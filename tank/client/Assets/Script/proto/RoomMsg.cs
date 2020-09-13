using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace Assets.Script.proto
//{
    class MsgGetAchieve: MsgBase
    {
        public MsgGetAchieve()
        {
            protoName = "MsgGetAchieve";
        }
        //服务器回
        public int winCount = 0;
        public int failCount = 0;
    }
    [Serializable]
    class RoomInfo
    {
        public int id = 0;
        //人数
        public int population = 0;
        /// <summary>
        /// 状态:0-准备中;1-战斗中;
        /// </summary>
        public int status = 0;  
    }
    class MsgGetRoomList : MsgBase
    {
        public MsgGetRoomList()
        {
            protoName = "MsgGetRoomList";
        }
        /// <summary>
        /// 房间列表,服务端回
        /// </summary>
        public RoomInfo[] roomList;
    }
    class MsgCreateRoom : MsgBase
    {
        public MsgCreateRoom()
        {
            protoName = "MsgCreateRoom";
        }
        ///// <summary>
        ///// 服务端回:0-成功;1-失败
        ///// </summary>
        //public int result = 0;
        /// <summary>
        /// 服务端回: -1 - 创建房间失败; > 0 -房间Id
        /// </summary>
        public int roomId = -1;
    }
    class MsgEnterRoom : MsgBase
    {
        public MsgEnterRoom()
        {
            protoName = "MsgEnterRoom";
        }
        /// <summary>
        /// 客户端发,要进入的房间的id
        /// </summary>
        public int id = -1;
        /// <summary>
        /// 客户端回,0表示成功
        /// </summary>
        public int result = 0;
    }
    [Serializable]
    class PlayerInfo
    {
        public int id = -1;
        public string name = "";
        /// <summary>
        /// 阵营
        /// </summary>
        public int camp = 0;
        public int winCount = 0;
        public int lostCount = 0;
        /// <summary>
        /// 房主
        /// </summary>
        public bool isOwner = false;
    }
    class MsgGetRoomInfo : MsgBase
    {
        public MsgGetRoomInfo()
        {
            protoName = "MsgGetRoomInfo";
        }
        public PlayerInfo[] players;
    }
    class MsgLeaveRoom : MsgBase
    {
        public MsgLeaveRoom()
        {
            protoName = "MsgLeaveRoom";
        }
        public int result = 0;
    }
    class MsgStartBattle : MsgBase
    {
        public MsgStartBattle()
        {
            protoName = "MsgStartBattle";
        }
        public int result = 0;
    }
//}
