using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Logic
{
    public class Grid
    {
        const int PRECISION = 2;
        const int GRIDSIZE = 1;
        public string Id { get; set; }
        public Geolocation Location { get; set; }
        public List<Geolocation> Points { get; set; }
        public Grid()
        {
            Location = new Geolocation();
            Points = new List<Geolocation>();
        }

        public Grid(string gridId) : this()
        {
            Id = gridId;
            string[] gridParts = gridId.Split('.');
            var lat = double.Parse(gridParts[0]);
            var lng = double.Parse(gridParts[1]);

            double pow = Math.Pow(10, PRECISION);
            lat = lat / pow;
            lng = lng / pow;

            GetGrid(lat, lng);
        }

        public void GetGrid(string location)
        {
            string[] locParts = location.Split(',');
            double lat = double.Parse(locParts[0].Trim());
            double lng = double.Parse(locParts[1].Trim());

            GetGrid(lat, lng);
        }

        public void GetGrid(double lat, double lng)
        {
            Location.Lat = lat;
            Location.Lng = lng;
            double pow = Math.Pow(10, PRECISION);
            lat = Math.Floor(lat * pow);
            lng = Math.Floor(lng * pow);
            Id = string.Format("{0}.{1}", lat.ToString(), lng.ToString());
            lat = lat / pow;
            lng = lng / pow;

            double delta = GRIDSIZE / pow;
            Points.Add(new Geolocation() { Lat = lat, Lng = lng });
            lat += delta;
            Points.Add(new Geolocation() { Lat = lat, Lng = lng });
            lng += delta;
            Points.Add(new Geolocation() { Lat = lat, Lng = lng });
            lat -= delta;
            Points.Add(new Geolocation() { Lat = lat, Lng = lng });
            lng -= delta;
            Points.Add(new Geolocation() { Lat = lat, Lng = lng });
        }

        public List<double> GetPerimeterGridIds(string gridId)
        {
            double srcGrid = double.Parse(gridId);
            List<double> periGrids = new List<double>();
            periGrids.Add(srcGrid - 1.0001);
            periGrids.Add(srcGrid - 1.0000);
            periGrids.Add(srcGrid - 0.9999);
            periGrids.Add(srcGrid - 0.0001);
            periGrids.Add(srcGrid + 0.0001);
            periGrids.Add(srcGrid + 0.9999);
            periGrids.Add(srcGrid + 1.0000);
            periGrids.Add(srcGrid + 1.0001);
            return periGrids;
        }
    }
}
