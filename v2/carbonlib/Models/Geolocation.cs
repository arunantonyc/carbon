namespace carbonlib.Models
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
