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
    public class StoreController : Controller
    {
        StoreRepo srepo = new StoreRepo();
        StockRepo repo = new StockRepo();
        // GET: Store
        public ActionResult Index()
        {
            var lstStores = srepo.GetAllStores();
            return View(lstStores);
        }

        //Add new Store
        public ActionResult AddStore()
        {
            tblStore objStore = new tblStore();
            return View(objStore);
        }

        public ActionResult SaveStore(tblStore store)
        {
            bool result = srepo.SaveStore(store);
            if(result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("AddStore");
            }
            
        }
        //Update Customer
        public ActionResult Details(int id)
        {
            var objStore = srepo.FindStoreById(id);
            return View("AddStore", objStore);
        }

        public ActionResult Delete(int id)
        {
           srepo.DeleteStoreById(id);
            return RedirectToAction("Index");
        }

        public ActionResult AjaxStoreListing()
        {
            return View();
        }

        public ActionResult AjaxStoreList()
        {
            StoreAndStock list = new StoreAndStock();
            list.lstStores = srepo.GetAllStores();
            list.lstStocks = repo.getAllProduct();
           

            return PartialView("_AjaxStoreList", list);
        }

        public ActionResult SaveStoreAjax(string Id,string StoreName, string Address, string City, string ContactNo)
        {
            tblStore objStore = new tblStore();
            if (!string.IsNullOrEmpty(Id))
            {
                objStore.Id = Convert.ToInt32(Id);
            }
            objStore.StoreName = StoreName;
            objStore.Address = Address;
            objStore.City = City;
            objStore.ContactNo = ContactNo;
            var result = srepo.SaveStore(objStore);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreById(string Id)
        {
           tblStore store = srepo.FindStoreById(Convert.ToInt32(Id));
            return Json(store, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteStore(string Id)
        {
            var result = srepo.DeleteStoreById(Convert.ToInt32(Id));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}