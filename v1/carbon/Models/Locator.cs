using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carbon.Models
{
    public class Locator
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public DateTime LastSeenOn { get; set; }
        public LocFeed LastSeenAt { get; set; }
    }
}
