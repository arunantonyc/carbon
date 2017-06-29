using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Logic
{
    public class Geolocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1}", Lat, Lng);
        }
    }

}
