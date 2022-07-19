using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Models;
using ShopNowBL.Repository;
using ShowNow.ViewModels;
using Rotativa;
using System.IO;
using System.Net.Mail;
using System.Net;


namespace ShowNow.Controllers
{
    public class TransactionController : Controller
    {
        TransactionRepo TR = new TransactionRepo();
        StockRepo SR = new StockRepo();
        CustomerRepo CR = new CustomerRepo();
        // GET: Transaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SaveTransaction(string Id, string TotalQty, string TotalAmount,string PayMethod,decimal totalDisc,decimal totalgst, List<tblTransactionItem> TItems)
        {
            //save transaction
            tblTransaction objData = new tblTransaction();
            objData.TotalQty = Convert.ToInt32(TotalQty);
            objData.InvoiceAmount = Convert.ToDecimal(TotalAmount);
            objData.PaymentMethod = PayMethod;
            objData.CustomerId = Convert.ToInt32(Id);
            objData.InvoiceNo = TR.GenerateId();
            objData.GST = totalgst;
            objData.TotalDiscount = totalDisc;
            var result = TR.SaveTransaction(objData);
            
            var itemResult = false;
            //save transaction Items
           // List<tblTransactionItem> lstItems = TItems;


           if (result)
            {
                
                foreach(tblTransactionItem T in TItems)
                {
                    tblTransactionItem objItem = new tblTransactionItem();
                    objItem.InvoiceId = objData.InvoiceNo;
                    objItem.ProductId = T.ProductId;
                    objItem.Price = T.Price;
                    objItem.Qty = T.Qty;
                    itemResult=TR.SaveTransactionItems(objItem);
                    //update stock
                    tblStock stock =  SR.FindProductById(objItem.ProductId);
                    stock.ProductQty -= Convert.ToInt32(objItem.Qty);
                    SR.saveProduct(stock);
                }
            }

            return Json(itemResult, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SaveTransactions(string Id, string TotalQty, string TotalAmount, string PayMethod, decimal totalDisc, decimal totalgst, List<tblTransactionItem> TItems)
        {
            //save transaction
            tblTransaction objData = new tblTransaction();
            objData.TotalQty = Convert.ToInt32(TotalQty);
            objData.InvoiceAmount = Convert.ToDecimal(TotalAmount);
            objData.PaymentMethod = PayMethod;
            objData.CustomerId = Convert.ToInt32(Id);
            objData.InvoiceNo = TR.GenerateId();
            objData.GST = totalgst;
            objData.TotalDiscount = totalDisc;
          
            List<tblTransactionItem> lstItems = TItems;

            tblTransaction result = TR.SaveTransactions(objData, lstItems);

         
            return Json(result, JsonRequestBehavior.AllowGet);

        }
       
        public ActionResult TransactionPage()
        {
            TransactionModel transactionModel = new TransactionModel();
            transactionModel.lstProduct = SR.getAllProduct();
            return View(transactionModel);
        }

        public ActionResult GetProductById(string Id)
        {
            tblStock stock = SR.FindProductById(Convert.ToInt32(Id));
            return Json(stock, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveTransactionViewModel(TransactionModel objData)
        {
            objData.objTransaction.InvoiceNo = TR.GenerateId();
            objData.objItem.InvoiceId = objData.objTransaction.InvoiceNo;
            objData.objItem.Qty = objData.objTransaction.TotalQty;

            TR.CaptureTransaction(objData.objItem,objData.objTransaction,objData.objCustmore);
            return RedirectToAction("TransactionPage");
        }

        public ActionResult BillPreview(string InvoiceNo,string hide, string receiver)
        {
            //InvoiceNo = "6cae7dcfd0e4607b";
            TransactionModel transactionModel = new TransactionModel();

            transactionModel.lstItems = TR.GetTransactionItemsByInvoiceNo(InvoiceNo);
            transactionModel.objTransaction = TR.GetTransactionByInvoiceNo(InvoiceNo);
            transactionModel.objCustmore = CR.getCustomerById(transactionModel.objTransaction.CustomerId);

            ViewBag.receiver = receiver;
            ViewBag.Hide = hide;

            return PartialView("_BillPreview",transactionModel);
        }

        public ActionResult PrintInvoice(string InvoiceNo,string receiver)
        {
            var a = new ViewAsPdf();
            a.ViewName = "_BillPreview";
            TransactionModel transactionModel = new TransactionModel();
            transactionModel.lstItems = TR.GetTransactionItemsByInvoiceNo(InvoiceNo);
            transactionModel.objTransaction = TR.GetTransactionByInvoiceNo(InvoiceNo);
            transactionModel.objCustmore = CR.getCustomerById(transactionModel.objTransaction.CustomerId);

            ViewBag.InvoiceId = InvoiceNo;
            ViewBag.Hide = 1;

            a.Model = transactionModel;
            var pdfBytes = a.BuildFile(this.ControllerContext);

            // Optionally save the PDF to server in a proper IIS location.
            var fileName = transactionModel.objCustmore.CustomerName+transactionModel.objTransaction.InvoiceNo + ".pdf";
            var path = Server.MapPath("~/Temp/" + fileName);
            //var path = "https://liacrm.com/CRM_Publish/Temp/" + fileName;
            System.IO.File.WriteAllBytes(path, pdfBytes);

            // return ActionResult
            MemoryStream ms = new MemoryStream(pdfBytes);

            
            //Email sending
            try
            {
                var senderEmail = new MailAddress("kartikgund2@gmail.com", "Kartik");
                var receiverEmail = new MailAddress(receiver, "Receiver");
                var password = "rgpljlpevjrezdpq";
                var sub = "Invoice For Your Purchase";
                var body = "Dear "+ transactionModel.objCustmore.CustomerName+ " Thank you for your purchase your Invoice is here, Visit Again!!";

                MailMessage message = new MailMessage();
                message.To.Add(receiver);// Email-ID of Receiver  
                message.Subject = sub;// Subject of Email  
                message.From = senderEmail;// Email-ID of Sender  
                message.IsBodyHtml = true;
                Attachment data = new Attachment(ms, fileName, "application/pdf");
                message.Attachments.Add(data);
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
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }

            return RedirectToAction("BuyProduct", "Stocks");
        }
    }
}