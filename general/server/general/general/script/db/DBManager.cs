using System;
using System.Collections.Generic;
using System.Text;
//using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

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
        public static bool IsAccountExist(string id)
        {
            //防止sql注入
            if (!DBManager.IsSafeString(id))
            {
                return false;
            }
            //sql语句
            string s = String.Format("select * from account where id = '{0}';", id);
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
                Console.WriteLine("[数据库] IsAccountExist err, " + e.Message);
                return false;
            }

        }
        private static bool IsSafeString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
    }
}
