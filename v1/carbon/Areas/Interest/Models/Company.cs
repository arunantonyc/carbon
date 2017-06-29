using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Areas.Interest.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
    }

    public class CompanyStores : Company
    {
        public List<Store> Stores { get; set; }
        public CompanyStores()
        {
            Stores = new List<Store>();
        }
    }
}
