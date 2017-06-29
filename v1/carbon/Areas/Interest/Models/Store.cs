using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace carbon.Areas.Interest.Models
{

    public class Store
    {
        public int StoreId { get; set; }
        public int CompanyId { get; set; }
        public string StoreName { get; set; }
        public string Landmark { get; set; }        
        public string Address { get; set; }

        [DisplayFormat(DataFormatString = "{0: h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime OpenTime { get; set; }

        [DisplayFormat(DataFormatString = "{0: h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime CloseTime { get; set; }
        public string Phone { get; set; }

        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Grid { get; set; }
        public double Distance { get; set; }
        public int Time { get; set; }
    }

    public class StoreLite
    {
        public int I { get; set; }
        public string N { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double G { get; set; }
        public double D { get; set; }
        public double T { get; set; }
        public string A { get; set; }
    }
}
