using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        StoreRepo SR = new StoreRepo();
        
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult RegisterUser()
        {
            UserAndStore userAndStore = new UserAndStore();
            userAndStore.lstStore = SR.GetAllStores();
            return View(userAndStore);
        }

        [HttpPost]
        public ActionResult SaveUser(UserAndStore userAndStore)
        {
            tblUser objAdmin = userAndStore.objUser;
            objAdmin.CreatedDate = DateTime.Now;
            objAdmin.CreatedBy = 1;
            objAdmin.RoleId = 2;
            objAdmin.Password = repo.encrypt(objAdmin.Password);
            bool result = repo.AddUser(objAdmin);
            if (result == true)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                return RedirectToAction("RegisterUser");
            }

        }
        public ActionResult AuthenticateUser(tblUser objUser)
        {
            string pass = repo.encrypt(objUser.Password);
            var user = repo.ValidateUser(objUser.EmailId, pass);
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

        public ActionResult ForgetPassword()
        {
            return View();
        }

        public ActionResult VerifyEmail(string email)
        {
            tblUser user = repo.VerifyEmail(email);
           
            bool result=false;
            if (user != null)
            {
                tblOTP objOtp = new tblOTP();
                Random r = new Random();
                int genRand = r.Next(100000, 999999);
                objOtp.EmailId = user.EmailId;
                objOtp.OTP = genRand;
                objOtp.IsUsed = 0;
                objOtp.Created_DateTime = DateTime.Now;
                result = repo.AddOtpToDb(objOtp);

                if (result)
                {
                    try
                    {
                        var senderEmail = new MailAddress("kartikgund2@gmail.com", "Kartik");
                        var receiverEmail = new MailAddress(user.EmailId, "Receiver");
                        var password = "rgpljlpevjrezdpq";
                        var sub = "OTP for Reset Password";
                        var body = "Dear " + user.UserName +  " <b>" + genRand + "</b> is your One-Time-Password. It is valid for 10 mins.";

                        MailMessage message = new MailMessage();
                        message.To.Add(user.EmailId);// Email-ID of Receiver  
                        message.Subject = sub;// Subject of Email  
                        message.From = senderEmail;// Email-ID of Sender  
                        message.IsBodyHtml = true;
                       
                        message.Body = body;
                        SmtpClient SmtpMail = new SmtpClient();

                        SmtpMail.Host = "smtp.gmail.com";
                        SmtpMail.Port = 587;
                        SmtpMail.EnableSsl = true;
                        SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                        SmtpMail.UseDefaultCredentials = false;
                        SmtpMail.Credentials = new NetworkCredential(senderEmail.Address, password);
                        SmtpMail.Send(message);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

            }
             return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyOTP(string email,string otp)
        {
            tblOTP objotp = repo.GetObjOtpByEmail(email);
            string result = "OTP is Used,Generate another OTP";

            TimeSpan ts = DateTime.Now - objotp.Created_DateTime;
         //   int time = Convert.ToInt32(ts);
            if (ts.Minutes <= 10)
            {
                 if (objotp.OTP == Convert.ToInt32(otp))
                 {
                    objotp.IsUsed = 1;
                    repo.AddOtpToDb(objotp);
                    result = "Valid OTP";
                 }
            }
            else
            {
                result = "OTP expired";
            }

           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetPassword(string email)
        {
            tblUser objUser = repo.VerifyEmail(email);
            ViewBag.EmailId = objUser.EmailId;
            return PartialView("_ResetPassword");
        }

        public ActionResult SavePassword(string email,string password1)
        {
            tblUser objUser = repo.VerifyEmail(email);
           
            objUser.Password = repo.encrypt(password1);
            repo.AddUser(objUser);
            return RedirectToAction("Login");
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
            objAdmin.CreatedDate = DateTime.Now;
            objAdmin.CreatedBy = 1;
            objAdmin.RoleId = 2;
            objAdmin.Password = repo.encrypt(objAdmin.Password);
            bool result = repo.AddUser(objAdmin);
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
            objAdmin.Password = repo.Decrypt(objAdmin.Password);
            return View("AddAdmin", objAdmin);
        }

        //List cashier
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
            objCashier.CreatedBy = 2;
            objCashier.CreatedDate = DateTime.Now;
            objCashier.RoleId = 3;
            bool result = repo.AddUser(objCashier);
            if (result == true)
            {
                return RedirectToAction("AdminList");
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }

        }

       
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
            user.CreatedDate = DateTime.Now;
            user.CreatedBy = 1;
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