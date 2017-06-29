using carbon.Areas.Interest.Models;
using carbon.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Data
{
    public class poiContext : DbContext
    {
        private DataTable getGridStores(double centerGrid, List<double> periGrids)
        {
            DBConnect db = new DBConnect();
            StringBuilder query = new StringBuilder(string.Format("SELECT s.StoreId, s.CompanyId, s.Name, s.Landmark, s.Address, s.OpenTime, s.CloseTime, s.Phone, l.StoreLocId, l.Latitude, l.Longitude, l.Grid FROM store s INNER JOIN store_location l ON s.storeid = l.storeid WHERE l.grid in ({0}", centerGrid));
            if ((periGrids != null) && (periGrids.Count > 0))
            {
                foreach (double gId in periGrids)
                {
                    query.Append(string.Format(", {0}", gId.ToString()));
                }
            }
            query.Append(");");

            return db.ExecuteRead(query.ToString());
        }


        public List<Store> GetPerimeterStores(string centerGridId)
        {
            Grid grid = new Grid();
            List<double> perimeterGridIds = grid.GetPerimeterGridIds(centerGridId);

            List<Store> stores = new List<Store>();
            DataRowCollection rows = getGridStores(double.Parse(centerGridId), perimeterGridIds).Rows;
            foreach (DataRow row in rows)
            {
                object[] item = row.ItemArray;
                double lat = Helpers.TryDouble(item[9]);
                double lng = Helpers.TryDouble(item[10]);
                double gridId = Helpers.TryDouble(item[11]);
                string loc = string.Format("{0},{1}", lat.ToString(), lng.ToString());
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
                    Grid = gridId,
                    Location = loc
                });
            }

            return stores;
        }
    }
}
