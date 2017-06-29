using carbonlib;
using carbonlib.Data;
using carbonlib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;

namespace carbon2.Controllers
{
    public class LinkController : ApiController
    {
        public string getCookie()
        {
            var cookies = this.Request.Headers.GetCookies("fms");
            if (cookies.Any())
            {
                var cookie = cookies.First().Cookies;
                if (cookie.Any())
                {
                    var value = cookie.First().Value;
                    var cookieParts = value.Split('-');
                    return cookieParts[0];
                }
            }
            return "";
        }

        public string Post([FromBody]object uKey)
        {
            if (uKey == null)
                return Helpers.JsonSerialize(new Payload() { Rst = -1 });
            try
            {
                UserLocLite currLogin = (UserLocLite)Helpers.JsonDesrialize(uKey.ToString(), typeof(UserLocLite));
                LinkContext dbUser = new LinkContext();
                UserLinks usrs = dbUser.GetLinkUsers(currLogin.k);
                if (usrs != null)
                {
                    usrs.Rst = 1;
                    var output = Helpers.JsonSerialize(usrs);
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
        /// <param name="linkAction">
        /// Contains status as follows:
        /// 1 - New
        /// 2 - Accept
        /// -1 - Reject
        /// 0 - Remove
        /// </param>
        /// <returns></returns>
        public string Put([FromBody]object linkAction)
        {
            if (linkAction == null)
                return Helpers.JsonSerialize(new Payload() { Rst = -1 });
            try
            {
                UserLink linkInfo = (UserLink)Helpers.JsonDesrialize(linkAction.ToString(), typeof(UserLink));
                LinkContext dbLink = new LinkContext();
                if (linkInfo.Status == 1)
                    dbLink.AddLinkUser(getCookie(), linkInfo.Key);

                if ((linkInfo.Status == 2) || (linkInfo.Status == -1))
                    dbLink.EditLinkStatus(getCookie(), linkInfo.Id, linkInfo.Status);

                if (linkInfo.Status == 0)
                    dbLink.DeleteLink(linkInfo.Id);

                return Helpers.JsonSerialize(new Payload() { Rst = 1 });
            }
            catch (Exception ex)
            {
                return Helpers.JsonSerialize(new Payload() { Rst = -1 });
            }
        }
    }
}
