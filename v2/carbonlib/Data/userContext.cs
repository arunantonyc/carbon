using carbonlib.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace carbonlib.Data
{
    public class UserContext
    {
        private DataTable verifyUser(string key, string password)
        {
            DbConnect db = new DbConnect();
            var query = "sp_userLogin";
            return db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                new MySqlParameter("@userKey", key),
                new MySqlParameter("@userPwd", password)
            });
        }
        private int addUser(string key, string password, string name)
        {
            try
            {
                DbConnect db = new DbConnect();
                var query = "sp_userAdd";
                db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                    new MySqlParameter("@userKey", key),
                    new MySqlParameter("@userPwd", password),
                    new MySqlParameter("@userName", name)
                });
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }         
        }
        private int updateUserLoc(string key, double xCoord, double yCoord, double grid)
        {
            try
            {
                DbConnect db = new DbConnect();
                var query = "sp_userUpdateLoc";
                db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                    new MySqlParameter("@uKey", key),
                    new MySqlParameter("@x", xCoord),
                    new MySqlParameter("@y", yCoord),
                    new MySqlParameter("@g", grid)
                });
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        
        public User AddUser(string key, string password, string name)
        {
            var rst = addUser(key, password, name);
            if (rst > 0)
                return VerifyUser(key, password);
            else
                return null;
        }
        public User VerifyUser(string key, string password)
        {
            DataRowCollection rows = verifyUser(key, password).Rows;
            if (rows.Count > 0)
            {
                object[] item = rows[0].ItemArray;
                
                User usr = new User()
                {
                    Id = int.Parse(item[0].ToString()),
                    Key = item[1].ToString(),
                    Name = item[2].ToString()                                       
                };
                try
                {
                    Geolocation loc = new Geolocation()
                    {
                        Lat = double.Parse(item[3].ToString()),
                        Lng = double.Parse(item[4].ToString())
                    };
                    usr.At = loc;
                }
                catch { }
                try
                {
                    usr.On = DateTime.Parse(item[6].ToString());
                }
                catch { }
                return usr;
            }
            return null;
        }
        public int UpdateUserLocation(string userKey, double xCoord, double yCoord, double gridId)
        {
            return updateUserLoc(userKey, xCoord, yCoord, gridId);
        }
        
    }
}
