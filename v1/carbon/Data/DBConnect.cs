using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Data
{
    public class DBConnect
    {
        public string _connStr;
        public DBConnect()
        {
            _connStr = ConfigurationManager.ConnectionStrings["fms"].ConnectionString;
        }

        public DataTable ExecuteRead(string query)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(_connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                        Console.WriteLine("Records: {0}", dt.Rows.Count);
                    }
                }
            }
            return dt;
        }

        public int ExecuteWrite(string query)
        {
            int rowsAffected;
            using (MySqlConnection con = new MySqlConnection(_connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                    
                    Console.WriteLine("Records: {0}", rowsAffected);
                }
            }
            return rowsAffected;
        }
    }
}
