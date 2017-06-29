using carbon.Areas.Interest.Models;
using carbon.Data;
using carbon.Logic;
using carbon.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace carbon.Controllers
{
    public class PoiController : ApiController
    {
        private poiContext db = new poiContext();
        // GET: api/Poi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Poi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Poi
    
        public string Post([FromBody] object latLng)
        {
            if (latLng == null)
                return "";
            try {

                Debug.WriteLine( latLng);

                LocFeed currLoc = (LocFeed)Helpers.JsonDesrialize(latLng.ToString(), typeof(LocFeed));
                Grid grid = new Grid();
                var lat = double.Parse(currLoc.x);
                var lng = double.Parse(currLoc.y);
                grid.GetGrid(lat, lng);

                GIS gis = new GIS();
                List<Store> stores = db.GetPerimeterStores(grid.Id); // "1296.7764");
                
                List<StoreLite> feed = new List<StoreLite>();
                foreach (Store store in stores)
                {
                    //store.Distance = Math.Truncate(Helpers.GetStraightDistance(lat, lng, store.Latitude, store.Longitude) * 100) / 100;

                    double dist, dura;
                    var result = gis.GetDistanceMatrix(lat, lng, store.Latitude, store.Longitude, out dist, out dura);
                    dist = Math.Truncate(dist / 100) / 10;
                    TimeSpan ts = new TimeSpan(0, 0, (int)dura);
                    dura = Math.Round(ts.TotalMinutes);
                    feed.Add(new StoreLite()
                    {
                        I = store.StoreId,      // id
                        N = store.StoreName,    // name
                        X = store.Latitude,     // lat
                        Y = store.Longitude,    // lng
                        G = store.Grid,         // grid
                        D = dist,               // distance
                        T = dura,               // time
                        A = store.Address       // address
                    });
                }
                
                var output = Helpers.JsonSerialize(feed.OrderBy(s => s.T).ThenBy(s => s.D));
                return output;
            }
            catch
            {
                return "";
            }
        }

        // PUT: api/Poi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Poi/5
        public void Delete(int id)
        {
        }
        
    }
}
