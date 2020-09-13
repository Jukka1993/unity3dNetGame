using System;
using System.Collections.Generic;
using System.Text;
using general.script.proto;
using general.script.net;

namespace general.script.logic
{
    class RoomManager
    {
        private static int maxId = 1;
        public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();
        public static Room GetRoom(int id)
        {
            if (rooms.ContainsKey(id))
            {
                return rooms[id];
            }
            return null;
        }
        public static Room AddRoom()
        {
            maxId++;
            Room room = new Room();
            room.roomId = maxId;
            rooms.Add(room.roomId, room);
            return room;
        }
        public static bool RemoveRoom(int id)
        {
            rooms.Remove(id);
            return true;
        }
        public static MsgBase ToMsg()
        {
            MsgGetRoomList msg = new MsgGetRoomList();
            int count = rooms.Count;
            msg.roomList = new RoomInfo[count];
            int i = 0;
            foreach(Room room in rooms.Values)
            {
                RoomInfo roomInfo = new RoomInfo();
                roomInfo.id = room.roomId;
                roomInfo.population = room.playerIds.Count;
                roomInfo.status = (int)room.status;

                msg.roomList[i] = roomInfo;
                i++;
            }
            return msg;
        }
    }
}
