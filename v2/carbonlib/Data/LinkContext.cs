using carbonlib.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace carbonlib.Data
{
    public class LinkContext
    {
        private int addLink(string key, string shareKey, int lnkType)
        {
            try
            {
                DbConnect db = new DbConnect();
                var query = "sp_linkAdd";
                db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                    new MySqlParameter("@uKey", key),
                    new MySqlParameter("@uKeyLink", shareKey),
                    new MySqlParameter("@lnkType", lnkType)
                });
                return 1;
            }
            catch (Exception ex)
            { return -1; }
        }
        private int editLinkStatus(string key, int linkId, int status)
        {
            try
            {
                DbConnect db = new DbConnect();
                var query = "sp_linkEditStatus";
                db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                    new MySqlParameter("@uKey", key),
                    new MySqlParameter("@lnkId", linkId),
                    new MySqlParameter("@uStatus", status)
                });
                return 1;
            }
            catch (Exception ex)
            { return -1; }
        }
        private int deleteLink(int linkId)
        {
            try
            {
                DbConnect db = new DbConnect();
                var query = "sp_linkDelete";
                db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                    new MySqlParameter("@lnkId", linkId)
                });
                return 1;
            }
            catch (Exception ex)
            { return -1; }
        }
        private DataTable getLinks(string key, int lnkType)
        {
            DbConnect db = new DbConnect();
            var query = "sp_linkGet";
            return db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                new MySqlParameter("@uKey", key),
                new MySqlParameter("@lnkType", lnkType)
            });
        }
        private DataTable getLinksRev(string key, int lnkType)
        {
            DbConnect db = new DbConnect();
            var query = "sp_linkGetReverse";
            return db.ExecuteParameterSelectCommand(query, CommandType.StoredProcedure, new MySqlParameter[] {
                new MySqlParameter("@uKey", key),
                new MySqlParameter("@lnkType", lnkType)
            });
        }


        public int AddLinkUser(string userKey, string shareKey)
        {
            return addLink(userKey, shareKey, 1);
        }
        public int EditLinkStatus(string userKey, int linkId, int status)
        {
            return editLinkStatus(userKey, linkId, status);
        }
        public int DeleteLink(int linkId)
        {
            return deleteLink(linkId);
        }
        public UserLinks GetLinkUsers(string userKey)
        {
            UserLinks links = new UserLinks();
            DataRowCollection rows = getLinks(userKey, 1).Rows;
            foreach (DataRow row in rows)
            {
                object[] item = row.ItemArray;
                UserLink usrLnk = new UserLink()
                {
                    LinkId = int.Parse(item[0].ToString()),
                    Id = int.Parse(item[5].ToString()),
                    Key = item[6].ToString(),
                    Name = item[7].ToString(),
                    Status = int.Parse(item[4].ToString())
                };
                try
                {
                    Geolocation loc = new Geolocation()
                    {
                        Lat = double.Parse(item[8].ToString()),
                        Lng = double.Parse(item[9].ToString())
                    };
                    usrLnk.At = loc;
                }
                catch { }
                try
                {
                    usrLnk.On = DateTime.Parse(item[11].ToString());
                }
                catch { }
                links.Users.Add(usrLnk);
            }
            return links;
        }        
    }
}
