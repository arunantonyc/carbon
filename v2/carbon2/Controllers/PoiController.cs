using carbonlib;
using carbonlib.Data;
using carbonlib.Loc;
using carbonlib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace carbon2.Controllers
{
    public class PoiController : ApiController
    {
        public string Put([FromBody] object latLng)
        {
            if (latLng == null)
                return "";
            try
            {
                Debug.WriteLine(latLng);

                UserLocLite currLoc = (UserLocLite)Helpers.JsonDesrialize(latLng.ToString(), typeof(UserLocLite));
                Grid grid = new Grid();
                var lat = double.Parse(currLoc.x);
                var lng = double.Parse(currLoc.y);
                grid.GetGrid(lat, lng);

                UserContext dbUser = new UserContext();
                if (!string.IsNullOrEmpty(currLoc.k))
                {
                    var rst = dbUser.UpdateUserLocation(currLoc.k, lat, lng, grid.Key);
                }
                return "";
            }
            catch(Exception ex)
            {
                return "";
            }
            
        }
    }
}
