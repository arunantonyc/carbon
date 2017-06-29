using carbon.Areas.Interest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace carbon.Data
{
    public class companyContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public companyContext() : base("name=companyContext")
        {
        }
        
        private DataTable getCompanies(int? id)
        {
            DBConnect db = new DBConnect();
            var query = "select * from company where status > 0";
            if (id != null)
                query = query + " and companyid = '" + id + "'";
            return db.ExecuteRead(query);
        }

        private DataTable getStores(int? companyId, int? storeId)
        {
            DBConnect db = new DBConnect();
            StringBuilder query = new StringBuilder("SELECT s.StoreId, s.CompanyId, s.Name, s.Landmark, s.Address, s.OpenTime, s.CloseTime, s.Phone, l.StoreLocId, l.Latitude, l.Longitude, l.Grid FROM store s LEFT OUTER JOIN store_location l ON s.storeid = l.storeid ");
            if ((companyId != null) || (storeId != null))
                query.Append(" where ");
            if (companyId != null)
            {
                query.Append(" s.companyId = '" + companyId + "'");
            }
            if (storeId != null)
            {
                if (companyId != null)
                    query.Append(" AND ");
                query.Append(" s.storeId = '" + storeId + "'");
            }
            query.Append(" ORDER BY s.storeId");
            return db.ExecuteRead(query.ToString());
        }

        
        public List<Company> Get(int? companyId)
        {
            DataRowCollection rows = getCompanies(companyId).Rows;

            List<Company> companies = new List<Company>();            
            foreach (DataRow row in rows)
            {
                object[] item = row.ItemArray;
                companies.Add(new Company() { CompanyId = int.Parse(item[0].ToString()), Name = item[1].ToString(), Title = item[2].ToString(), Status = int.Parse(item[3].ToString()) });
            }
            return companies;
        }

        public CompanyStores GetStores(int? companyId)
        {
            DataRowCollection rows = getCompanies(companyId).Rows;
            
            object[] company = rows[0].ItemArray;
            CompanyStores compStores = new CompanyStores()
            {
                CompanyId = int.Parse(company[0].ToString()),
                Name = company[1].ToString(),
                Title = company[2].ToString(),
                Status = int.Parse(company[3].ToString())
            };

            rows = getStores(companyId, null).Rows;
            foreach (DataRow row in rows)
            {
                object[] item = row.ItemArray;
                compStores.Stores.Add(new Store()
                {
                    StoreId = int.Parse(item[0].ToString()),
                    CompanyId = int.Parse(item[1].ToString()),
                    StoreName = item[2].ToString(),
                    Landmark = item[3].ToString(),
                    Address = item[4].ToString(),
                    OpenTime = DateTime.Parse(item[5].ToString()),
                    CloseTime = DateTime.Parse(item[6].ToString()),
                    Phone = item[7].ToString()
                });
            }            
            return compStores;
        }

        

        public int Add(Company comp)
        {
            DBConnect db = new DBConnect();
            var query = string.Format("insert into company(name, title, status) values('{0}','{1}',1)",
                comp.Name.Replace("'", "''"), comp.Title.Replace("'", "''"));
            return db.ExecuteWrite(query);            
        }
        public int Edit(Company comp)
        {
            DBConnect db = new DBConnect();
            var query = string.Format("UPDATE company SET Name='{1}', Title ='{2}' WHERE CompanyId={0}",
                comp.CompanyId, comp.Name.Replace("'", "''"), comp.Title.Replace("'", "''"));
            return db.ExecuteWrite(query);
        }
        public int Delete(int id)
        {
            DBConnect db = new DBConnect();
            // a delete is only a soft delete where the status is set as 0;
            var query = string.Format("UPDATE company SET Status = 0 WHERE CompanyId={0}", id);
            return db.ExecuteWrite(query);
        }


        public List<Store> GetStore(int? storeId)
        {
            List<Store> stores = new List<Store>();
            DataRowCollection rows = getStores(null, storeId).Rows;
            foreach (DataRow row in rows)
            {
                object[] item = row.ItemArray;
                double lat = tryDouble(item[9]);
                double lng = tryDouble(item[10]);
                double grid = tryDouble(item[11]);
                string loc = string.Format("{0}, {1}", lat.ToString(), lng.ToString());
                stores.Add(new Store()
                {
                    StoreId = int.Parse(item[0].ToString()),
                    CompanyId = int.Parse(item[1].ToString()),
                    StoreName = item[2].ToString(),
                    Landmark = item[3].ToString(),
                    Address = item[4].ToString(),
                    OpenTime = DateTime.Parse(item[5].ToString()),
                    CloseTime = DateTime.Parse(item[6].ToString()),
                    Phone = item[7].ToString(),
                    Latitude = lat,
                    Longitude = lng,
                    Grid = grid,
                    Location = loc
                });
            }
            return stores;
        }

        public int AddStore(Store store)
        {
            DBConnect db = new DBConnect();
            StringBuilder query = new StringBuilder();
            query.Append(string.Format("INSERT into store(companyId, name, landmark, address, opentime, closeTime, phone) VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}');",
                store.CompanyId, store.StoreName, store.Landmark, store.Address, store.OpenTime.TimeOfDay.ToString(@"hh\:mm\:ss"), store.CloseTime.TimeOfDay.ToString(@"hh\:mm\:ss"), store.Phone));
            
            //query.Append(string.Format("INSERT INTO store_location (StoreId, Latitude, Longitude, Grid) VALUES ({0}, {1}, {2}, {3});",
            //    store.StoreId, store.Latitude, store.Longitude, store.Grid));
            return db.ExecuteWrite(query.ToString());
        }
        public int EditStore(Store store)
        {
            DBConnect db = new DBConnect();
            StringBuilder query = new StringBuilder();
            query.Append(string.Format("UPDATE store SET Name='{1}', Landmark='{2}', Address='{3}', OpenTime='{4}', CloseTime='{5}', Phone='{6}' WHERE StoreId={0};",
                store.StoreId, store.StoreName.Replace("'", "''"), store.Landmark.Replace("'", "''"), store.Address.Replace("'", "''"), store.OpenTime.TimeOfDay.ToString(@"hh\:mm\:ss"), store.CloseTime.TimeOfDay.ToString(@"hh\:mm\:ss"), store.Phone));
            query.Append(string.Format("DELETE FROM store_location WHERE StoreId={0};", 
                store.StoreId));
            query.Append(string.Format("INSERT INTO store_location (StoreId, Latitude, Longitude, Grid) VALUES ({0}, {1}, {2}, {3});",
                store.StoreId, store.Latitude, store.Longitude, store.Grid));
            return db.ExecuteWrite(query.ToString());
        }
        public int DeleteStore(int storeId)
        {
            DBConnect db = new DBConnect();
            StringBuilder query = new StringBuilder();
            // a delete will remove the record from the database;
            query.Append(string.Format("DELETE FROM store_location WHERE StoreId={0};", storeId));
            query.Append(string.Format("DELETE FROM store WHERE StoreId={0}", storeId));

            return db.ExecuteWrite(query.ToString());
        }
        
        private double tryDouble(object item)
        {
            try
            {
                return double.Parse(item.ToString());
            }
            catch { return 0; }
        }
    }
}
