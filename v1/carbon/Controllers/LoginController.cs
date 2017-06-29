using carbon.Data;
using carbon.Logic;
using carbon.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace carbon.Controllers
{
    public class LoginController : ApiController
    {
        public void setCookie(string key)
        {
            var cookie = new HttpCookie("fms", key); 
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private locatorContext db = new locatorContext();
        // GET: api/Login
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Login/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Login
        public string Post([FromBody]object login)
        {
            if (login == null)
                return "";
            try
            {
                loginFeed currLogin = (loginFeed)Helpers.JsonDesrialize(login.ToString(), typeof(loginFeed));
                setCookie(currLogin.k);

                return "";
            }
            catch
            {
                return "";
            }
        }

        // PUT: api/Login/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Login/5
        public void Delete(int id)
        {
        }
    }
}
