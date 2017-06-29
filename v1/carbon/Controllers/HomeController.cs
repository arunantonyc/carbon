using carbon.Data;
using carbon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace carbon.Controllers
{
    public class HomeController : Controller
    {
        public string getCookie()
        {
            var cookie = Request.Cookies["fms"];
            if (cookie != null)
            {
                return cookie.Value;
            }
            return "";
        }
        public void setCookie(string key)
        {
            var cookie = Request.Cookies["fms"];
            cookie = new HttpCookie("fms", key);// Guid.NewGuid().ToString("N"));
            cookie.Expires.AddMinutes(15);
            Response.AppendCookie(cookie);
        }

        public ActionResult Index()
        {
            ViewBag.Title = "FindMySale Home";
            
            return View();
        }

        public ActionResult Location()
        {
            ViewBag.Title = "Location Page";

            return View();
        }

        public ActionResult LocationPoi() //Location based Points of Interest
        {
            ViewBag.Title = "Interests Page";
            //if (getCookie().Length < 1)
            //    return RedirectToAction("Register");
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Title = "Register Page";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Key, Name")] Locator @locator)
        {
            if (ModelState.IsValid)
            {
                if (@locator.Key == null)
                {
                    setCookie("guest");
                    return RedirectToAction("LocationPoi");
                }
                if (@locator.Key.Length > 0)
                {
                    locatorContext db = new locatorContext();
                    Locator loctr = db.GetLocatorInfo(@locator.Key);
                    if (loctr == null)
                        loctr = db.AddLocator(@locator);
                    setCookie(@locator.Key);
                }
                else
                    setCookie("guest");
            }
            else
                setCookie("guest");
            return RedirectToAction("LocationPoi");
        }
        
    }
}
