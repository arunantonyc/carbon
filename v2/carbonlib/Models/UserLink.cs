using System.Collections.Generic;

namespace carbonlib.Models
{
    public class UserLink : User
    {
        public int LinkId { get; set; }
        public int Status { get; set; }
        public int Direction { get; set; }
    }
    public class UserLinks : Payload
    {
        public List<UserLink> Users { get; set; }

        public int Count{ get { return Users.Count; } }

        public UserLinks()
        {
            Users = new List<UserLink>();
        }
    }
}
