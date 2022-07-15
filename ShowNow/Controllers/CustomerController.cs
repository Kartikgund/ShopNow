using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Models;
using ShopNowBL.Repository;

namespace ShowNow.Controllers
{
    public class CustomerController : Controller
    {
        CustomerRepo repo = new CustomerRepo();
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        //get customers
        public ActionResult ListCustomer()
        {
            var customers = repo.getAllCustomers();
            return View(customers);
        }

        //add customers
        public ActionResult AddCustomer()
        {
            tblCustomer objCustomer = new tblCustomer();
            return View(objCustomer);
        }
        [HttpPost]
        public ActionResult Create(tblCustomer objCustomer)
        {
            Boolean result = repo.addCustomer(objCustomer);
            if (result == true)
            {
                return RedirectToAction("ListCustomer");
            }
            else
            {
                return RedirectToAction("AddCustomer");
            }

        }

        //Update Customer
        public ActionResult Details(int id)
        {
            var objCustomer = repo.getCustomerById(id);
            return View("AddCustomer", objCustomer);
        }

        public ActionResult Delete(int id)
        {
            var objCustomer = repo.DeleteCustomerById(id);
            return RedirectToAction("ListCustomer");
        }

        public ActionResult SaveCustomerAjax(string Id,string CustomerName, string MobileNo)
        {
            tblCustomer objData = new tblCustomer();
            if (!string.IsNullOrEmpty(Id))
            {
                objData.Id = Convert.ToInt32(Id);
            }
           
            objData.CustomerName = CustomerName;
            objData.MobileNo = MobileNo;
            var result = repo.addCustomer(objData);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCustomerById(string Id)
        {
            tblCustomer customer = repo.getCustomerById(Convert.ToInt32(Id));
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCustomer(string Id)
        {
            var result = repo.DeleteCustomerById(Convert.ToInt32(Id));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerByNumber(string MobileNo)
        {
            tblCustomer customer = repo.getCustomerByMobileNo(MobileNo);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
    }
}