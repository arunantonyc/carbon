using System;

namespace carbonlib.Models
{
    public class User : Payload
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public DateTime On { get; set; }
        public Geolocation At { get; set; }
    }
}
