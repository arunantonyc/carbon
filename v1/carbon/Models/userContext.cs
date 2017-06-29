using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace carbon.Models
{
    public class userContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public userContext() : base("name=userContext")
        {
            Users = new List<Areas.Admin.Models.User>();

            // sample
            Users.Add(new Areas.Admin.Models.User()
            {
                Id = 1,
                Name = "Arun",
                Login = "9876543210"
            });
        }

        public List<carbon.Areas.Admin.Models.User> Users { get; set; }
    }
}
