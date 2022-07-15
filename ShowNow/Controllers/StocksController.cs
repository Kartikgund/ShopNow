using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Repository;
using ShopNowBL.Models;
using Newtonsoft.Json;

namespace ShowNow.Controllers
{
    public class StocksController : Controller
    {
        // GET: Stock
        StockRepo srepo = new StockRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListProduct()
        {
            var listStocks = srepo.getAllProduct();
            return View(listStocks);
        }

        public ActionResult AddProduct()
        {
            tblStock product = new tblStock();
            return View(product);
        }

        public ActionResult SaveProduct(tblStock product)
        {
            srepo.saveProduct(product);
            return RedirectToAction("ListProduct");
        }

        //Update Customer
        public ActionResult Details(int id)
        {
            var objStoke = srepo.FindProductById(id);
            return View("AddProduct", objStoke);
        }

        public ActionResult Delete(int id)
        {
            srepo.DeleteProductById(id);
            return RedirectToAction("ListProduct");
        }

        public ActionResult StockAjaxListing()
        {
            return View();
        }

        public JsonResult GetAjaxProductList()
        {
            var lstProduct = srepo.getAllProduct();
           // var jsonlstProduct = JsonConvert.SerializeObject(lstProduct);
            return Json(new { data= lstProduct }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveStockAjax(string Id,string ProductName, int Quantity, decimal BasePrice, decimal SellingPrice, int Discount)
        {
            tblStock objStock = new tblStock();
            if (!string.IsNullOrEmpty(Id))
            {
                objStock.Id = Convert.ToInt32(Id);
            }
            objStock.ProductName = ProductName;
            objStock.ProductQty = Quantity;
            objStock.BasePrice = BasePrice;
            objStock.SellingPrice = SellingPrice;
            objStock.Discount = Discount;
            var result = srepo.saveProduct(objStock);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductById(string Id)
        {
            tblStock stock = srepo.FindProductById(Convert.ToInt32(Id));
            return Json(stock, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProduct(string Id)
        {
            var result = srepo.DeleteProductById(Convert.ToInt32(Id));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuyProduct()
        {
            return View();
        }

    }
}