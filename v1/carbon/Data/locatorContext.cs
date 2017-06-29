using carbon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Data
{
    public class locatorContext
    {
        private DataTable getLocator(string key)
        {
            DBConnect db = new DBConnect();
            var query = string.Format("select * from user where UserKey = '{0}' and status = 1",
                key.Replace("'", "''"));
            return db.ExecuteRead(query);
        }

        public int insertLocator(string key, string name)
        {
            DBConnect db = new DBConnect();            
            var query = string.Format("insert into user(UserKey, UserName, Status) values('{0}','{1}', 1)",
                key.Replace("'", "''"), name.Replace("'", "''"));
            return db.ExecuteWrite(query);
        }

        public Locator AddLocator(Locator loctr)
        {
            insertLocator(loctr.Key, loctr.Name);
            return loctr;
        }
        public Locator GetLocatorInfo(string key)
        {
            DataRowCollection rows = getLocator(key).Rows;
            if (rows.Count > 0)
            {                
                object[] item = rows[0].ItemArray;
                Locator loctr = new Locator()
                {
                    Key = item[0].ToString(),
                    Name = item[1].ToString()
                };
                return loctr;
            }
            return null;
        }
    }
}
