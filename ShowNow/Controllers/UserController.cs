using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Models;
using ShopNowBL.Repository;
using ShowNow.ViewModels;

namespace ShowNow.Controllers
{
    public class UserController : Controller
    {
        UserRepo repo= new UserRepo();
        CustomerRepo CR = new CustomerRepo();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AuthenticateUser(tblUser objUser)
        {
            var user = repo.ValidateUser(objUser.EmailId, objUser.Password);
            if(user!=null)
            {
                Session["User"] = user;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.InvalidUser = "Invalid User";
                return View("Login");
            }
            
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult AdminList()
        {
            var admin = repo.GetAllAdmin();
            return View(admin);
        }

        //Add new Admin
        public ActionResult AddAdmin()
        {
            tblUser objAdmin = new tblUser();
            return View(objAdmin);
        }

        [HttpPost]
        public ActionResult SaveAdmin(tblUser objAdmin)
        {
            bool result = repo.AddAdmin(objAdmin);
            if (result == true)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("AddAdmin");
            }

        }

        public ActionResult AdminDetails(int id)
        {
            var objAdmin = repo.GetUserDetails(id);
            return View("AddAdmin", objAdmin);
        }

        public ActionResult AdminLogin()
        {
            var cashiers = repo.GetAllCashiers();
            return View(cashiers);
        }

        //Add new Cashier
        public ActionResult AddCashier()
        {
            tblUser objCashier = new tblUser();
            return View(objCashier);
        }

        [HttpPost]
        public ActionResult SaveCashier(tblUser objCashier)
        {
            bool result = repo.AddCashier(objCashier);
            if (result == true)
            {
                return RedirectToAction("AdminList");
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }

        }

        //Delete Cashier
        public ActionResult DeleteAdmin(int id)
        {
            var result = repo.DeleteUserById(id);
            return RedirectToAction("AdminList");
        }

        //Delete Cashier
        public ActionResult Delete(int id)
        {
            var result = repo.DeleteUserById(id);
            return RedirectToAction("AdminLogin");
        }

        //Update Cashier
        public ActionResult Details(int id)
        {
            var objCashier = repo.GetUserDetails(id);
            return View("AddCashier", objCashier);
        }

        public ActionResult ListUserCustomer()
        {
            UserAndCustomer UC = new UserAndCustomer();
            UC.lstCustomers = CR.getAllCustomers();
            UC.lstUsers = repo.GetAllUsers();
            UC.customerCount = UC.lstCustomers.Count;
            UC.userCount = UC.lstUsers.Count;
            return View(UC);
        }

        public ActionResult AjaxPartialListing()
        {
            UserAndCustomer UC = new UserAndCustomer();
            UC.lstCustomers = CR.getAllCustomers();
            UC.lstUsers = repo.GetAllUsers();
            UC.customerCount = UC.lstCustomers.Count;
            UC.userCount = UC.lstUsers.Count;

            return PartialView("_AjaxListing", UC);
        }

        public ActionResult AjaxListing()
        {
            return View();
        }

        
        public ActionResult SaveUserAjax(string Id,string UName, string MobileNo, string Email, string password, string city, int storeId,int RoleId)
        {

            tblUser user = new tblUser();
            if (!string.IsNullOrEmpty(Id))
            {
                user.Id = Convert.ToInt32(Id);
            }
            user.UserName = UName;
            user.MobileNo = MobileNo;
            user.EmailId = Email;
            user.Password = password;
            user.City = city;
            user.StoreId = storeId;
            user.RoleId = RoleId;
            var result = repo.AddUser(user);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserById(string Id)
        {
            tblUser user = repo.GetUserDetails(Convert.ToInt32(Id));
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteUser(string Id)
        {
            var result = repo.DeleteUserById(Convert.ToInt32(Id));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}