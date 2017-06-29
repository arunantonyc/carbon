using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace carbonlib.Data
{
    public class DbConnect
    {
        public string _connStr;
        public DbConnect()
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

        public DataTable ExecuteParameterSelectCommand(string cmdName, CommandType cmdType, MySqlParameter[] param)
        {
            try
            {
                DataTable dt = new DataTable();
                using (MySqlConnection con = new MySqlConnection(_connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(cmdName))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = cmdType;
                        cmd.Parameters.AddRange(param);
                        using (MySqlDataAdapter sda = new MySqlDataAdapter())
                        {
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
