using carbonlib;
using carbonlib.Data;
using carbonlib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace carbon2.Controllers
{
    public class LoginController : ApiController
    {
        public void setCookie(string key)
        {
            var cookie = new HttpCookie("fms", key);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public string Post([FromBody]object login)
        {
            if (login == null)
                return "";
            try
            {
                LoginLite currLogin = (LoginLite)Helpers.JsonDesrialize(login.ToString(), typeof(LoginLite));
                UserContext dbUser = new UserContext();
                User usr = dbUser.VerifyUser(currLogin.k, currLogin.p);
                if (usr != null)
                {
                    usr.Rst = 1;
                    var output = Helpers.JsonSerialize(usr);
                    //setCookie(currLogin.k);
                    //var loginresult = "{\"rst\": \"s\", \"k\": \"" + usr.Key + "\",\"n\": \"" + usr.Name + "\"}";
                    return output;
                }
                return Helpers.JsonSerialize(new Payload() { Rst = 0 });
            }
            catch (Exception ex)
            {
                return Helpers.JsonSerialize(new Payload() { Rst = -1 });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public string Put([FromBody]object register)
        {
            if (register == null)
                return "";
            try
            {
                LoginLite currRegister = (LoginLite)Helpers.JsonDesrialize(register.ToString(), typeof(LoginLite));
                UserContext dbUser = new UserContext();
                User usr = dbUser.AddUser(currRegister.k, currRegister.p, currRegister.n);
                if (usr != null)
                {
                    usr.Rst = 1;
                    var output = Helpers.JsonSerialize(usr);
                    //setCookie(currLogin.k);
                    //var loginresult = "{\"rst\": \"s\", \"k\": \"" + usr.Key + "\",\"n\": \"" + usr.Name + "\"}";
                    return output;
                }
                return Helpers.JsonSerialize(new Payload() { Rst = 0 });                
            }
            catch (Exception ex)
            {
                return Helpers.JsonSerialize(new Payload() { Rst = -1 });
            }
        }
    }
}
