using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Theater.DataBase
{
    public class DBmanager
    {
        public static MySqlConnection GetConnection(string server = "148.253.208.189", string database = "Theater", string userId = "TheaterUser", string password = "1")
        {
            string _connectionString = $"Server={server};Port=3306;Database={database};Uid={userId};Pwd={password};";
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

    }
}
