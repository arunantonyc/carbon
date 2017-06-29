using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Areas.Interest.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public int StoreId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; } 
    }
}
