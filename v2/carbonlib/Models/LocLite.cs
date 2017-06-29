namespace carbonlib.Models
{
    public class LocLite
    {
        public string x { get; set; }
        public string y { get; set; }
        public string r { get; set; }
    }

    public class UserLocLite : LocLite
    {
        public string k { get; set; }
    }
}
