using System;
using System.Collections.Generic;
using System.Text;

namespace general.script.logic
{
    class PlayerManager
    {
        public static Dictionary<int, Player> players = new Dictionary<int, Player>();
        //public static Dictionary<int, Player>.ValueCollection.Enumerator playersEnumerator
        //{
        //    get
        //    {
        //        //return players.Values.GetEnumerator();
        //        return players.Values.GetEnumerator();
        //    }
        //}
        public static bool IsOnline(int id)
        {
            return players.ContainsKey(id);
        }
        public static Player GetPlayer(int id)
        {
            if (players.ContainsKey(id))
            {
                return players[id];
            }
            return null;
        }
        public static void AddPlayer(int id,Player player)
        {
            players.Add(id, player);
        }
        public static void RemovePlayer(int id)
        {
            players.Remove(id);
        }
    }
}
