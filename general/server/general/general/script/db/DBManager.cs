using System;
using System.Collections.Generic;
using System.Text;
//using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using general.script.logic;

namespace general.script.db
{
    class DBManager
    {
        public static MySqlConnection mysql;
        //
        public static bool Connect(string db, string ip, int port, string user, string pw)
        {
            mysql = new MySqlConnection();
            string s = string.Format("Database= {0};Data Source={1};port={2};User={3};Password={4}", db, ip, port, user, pw);
            mysql.ConnectionString = s;
            //
            try
            {
                mysql.Open();
                Console.WriteLine();
                Console.WriteLine("[数据库]connect succ ");
                return true;
            }catch(Exception e)
            {
                Console.WriteLine("[数据库]connect fail, " + e.Message);
                return false;
            }
        }
        public static bool IsNameExist(string name)
        {
            //防止sql注入
            if (!IsSafeString(name))
            {
                return false;
            }
            //sql语句
            string s = String.Format("select * from account where name = '{0}';", name);
            //查询
            try
            {
                MySqlCommand cmd = new MySqlCommand(s, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return !hasRows;
            }catch(Exception e)
            {
                Console.WriteLine("[数据库] IsNameExist err, " + e.Message);
                return false;
            }

        }
        public static bool CreatePlayer(int id)
        {
            //
            PlayerData playerData = new PlayerData();
            string data = JsonConvert.SerializeObject(playerData);
            //
            string sql = string.Format("insert into player set id = '{0}',data = '{1}'", id, data);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return true;
            }catch(Exception e)
            {
                Console.WriteLine("[数据库] CreatePlayer err, " + e.Message);
                return false;
            }

        }
        public static bool UpdatePlayerData(int id,PlayerData playerData)
        {
            string data = JsonConvert.SerializeObject(playerData);
            string sql = string.Format("update player set data = '{0}' where id = '{1}';", data, id);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return true;
            } catch(Exception e)
            {
                Console.WriteLine("[数据库] UpdatePlayerData err, " + e.Message);
                return false;
            }
        }
        public static int CheckPassword(string name, string pw)
        {
            if (!IsSafeString(name))
            {
                Console.WriteLine("[] CheckPassword fail, name not safe");
                return -1;
            }
            if (!IsSafeString(pw))
            {
                Console.WriteLine("[]CheckPassword fail, pw not safe");
                return -1;
            }
            string sql = string.Format("select * from account where name = '{0}' and pw = '{1}';", name, pw);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                if (hasRows)
                {
                    dataReader.Read();
                    int id = dataReader.GetInt32("id");
                    dataReader.Close();
                    return id;
                } else
                {
                    dataReader.Close();
                    return -1;
                }
                //return hasRows;
            }catch(Exception e)
            {
                Console.WriteLine("[] CheckPassword err, " + e.Message);
                return -1;
            }
        }
        public static bool Register(string name,string pw)
        {
            //name防sql注入
            if (!IsSafeString(name))
            {
                Console.WriteLine("[数据库]Register fail, id not safe ");
                return false;
            }
            //pw防sql注入
            if (!IsSafeString(pw))
            {
                Console.WriteLine("[数据库]Register fail, pw not safe ");
                return false;
            }
            //能否注册，name是否已存在
            if (!IsNameExist(name))
            {
                Console.WriteLine("[数据库]Register fail, name exist");
                return false;
            }
            //写入数据库User表
            string sql = string.Format("insert into account set name = '{0}',pw = '{1}';", name, pw);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                cmd.ExecuteNonQuery();
                return true;
            } catch(Exception e)
            {
                Console.WriteLine("[数据库]Register fail " + e.Message);
                return false;
            }
        }
        public static int GetId(string name)
        {
            return 0;
        }
        public static PlayerData GetPlayerData(int id)
        {
            string sql = String.Format("select * from player where id = '{0}';", id);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, mysql);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    return null;
                } else
                {
                    reader.Read();
                    string datastr = reader.GetString("data");
                    PlayerData playerData = (PlayerData)JsonConvert.DeserializeObject(datastr, typeof(PlayerData));

                    reader.Close();
                    return playerData;

                }
            }catch (Exception e)
            {
                Console.WriteLine("[数据库] GetPlayerData fail, " + e.Message);
                return null;
            }
        }
        private static bool IsSafeString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
    }
}
