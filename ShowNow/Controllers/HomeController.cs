using ShopNowBL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Repository;

namespace ShowNow.Controllers
{
    public class HomeController : Controller
    {
        UserRepo repo = new UserRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login(string username, string password)
        {
            tblUser user = repo.ValidateUser(username, password);
            if (user!=null)
            {
                return RedirectToAction("AdminLogin", "User");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

      
    }
}