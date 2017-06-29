using carbonlib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace carbonlib.Loc
{
    public class GIS
    {
        public bool GetDistanceMatrix(double lat1, double lng1, double lat2, double lng2, out double distance, out double duration)
        {
            string url = string.Format("https://maps.googleapis.com/maps/api/distancematrix/json?origins={0},{1}&destinations={2},{3}",
                lat1.ToString(), lng1.ToString(), lat2.ToString(), lng2.ToString());
            WebRequest req = HttpWebRequest.Create(url);
            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            try
            {
                var str = sr.ReadToEnd();
                var jsonObj = Helpers.JsonDesrialize(str);
                Dictionary<string, object> results = (Dictionary<string, object>)jsonObj;
                var rows = ((object[])results["rows"])[0];
                var elements = ((Dictionary<string, object>)rows)["elements"];
                var element = ((object[])elements)[0];
                var pairs = ((Dictionary<string, object>)element);
                var dist = (Dictionary<string, object>)pairs["distance"];
                var dura = (Dictionary<string, object>)pairs["duration"];

                distance = double.Parse(dist["value"].ToString());
                duration = double.Parse(dura["value"].ToString());
                return true;
            }
            catch
            {
                distance = duration = 0;
                return false;
            }
            finally
            {
                sr.Close();
            }
        }

        public static double GetStraightDistance(double lat1, double lng1, double lat2, double lng2)
        {
            var p = 0.017453292519943295;    // Math.PI / 180

            var a = 0.5 - Math.Cos((lat2 - lat1) * p) / 2 +
                    Math.Cos(lat1 * p) * Math.Cos(lat2 * p) *
                    (1 - Math.Cos((lng2 - lng1) * p)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
        }

        public Geolocation GetGeoCode(string address)
        {
            string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}",
                address);
            WebRequest req = HttpWebRequest.Create(url);
            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            try
            {
                var str = sr.ReadToEnd();
                var jsonObj = Helpers.JsonDesrialize(str);
                Dictionary<string, object> results = (Dictionary<string, object>)jsonObj;
                var rows = ((object[])results["results"])[0];
                var elements = ((Dictionary<string, object>)rows)["geometry"];
                var element = ((Dictionary<string, object>)elements)["location"];
                var pairs = ((Dictionary<string, object>)element);
                var lat = pairs["lat"];
                var lng = pairs["lng"];
                return new Geolocation() { Lat = double.Parse(lat.ToString()), Lng = double.Parse(lng.ToString()) };
            }
            catch
            {
                return new Geolocation() { Lat = 0, Lng = 0 };
            }
            finally
            {
                sr.Close();
            }
        }
    }
}
