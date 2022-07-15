using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNowBL.Models;
using System.Data.Entity.Migrations;
using ShopNowBL.Repository;


namespace ShopNowBL.Repository
{
    public class TransactionRepo
    {
        StockRepo SR = new StockRepo();
        public bool SaveTransaction(tblTransaction transaction)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                // user.RoleId = 2;
                transaction.InvoiceDate = DateTime.Now;
                transaction.CreatedDate = DateTime.Now;
                transaction.CreatedBy = 4;
                context.tblTransactions.AddOrUpdate(transaction);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }

        public string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public bool SaveTransactionItems(tblTransactionItem TItem)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                context.tblTransactionItems.AddOrUpdate(TItem);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }


        public tblTransaction SaveTransactions(tblTransaction transaction, List<tblTransactionItem> TItems)
        {
           // bool Result = false;
            try
            {
                bool TResult = false;
                using (DPTContext context = new DPTContext())
                {
                    // user.RoleId = 2;
                    transaction.InvoiceDate = DateTime.Now;
                    transaction.CreatedDate = DateTime.Now;
                    transaction.CreatedBy = 4;
                    context.tblTransactions.AddOrUpdate(transaction);
                    context.SaveChanges();
                    TResult = true;
                }
                
                if (TResult)
                {

                    foreach (tblTransactionItem T in TItems)
                    {
                        tblTransactionItem objItem = new tblTransactionItem();
                        objItem.InvoiceId = transaction.InvoiceNo;
                        objItem.ProductId = T.ProductId;
                        objItem.Price = T.Price;
                        objItem.Qty = T.Qty;
                        using (DPTContext context = new DPTContext())
                        {
                            context.tblTransactionItems.AddOrUpdate(objItem);
                            context.SaveChanges();
                        }

                        //update stock
                        tblStock stock = SR.FindProductById(objItem.ProductId);
                        stock.ProductQty -= Convert.ToInt32(objItem.Qty);
                        SR.saveProduct(stock);
                     //   Result = true;
                    }
                    
                }
               
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
                return transaction;
            
        }

        public bool CaptureTransaction(tblTransactionItem objItem,tblTransaction objTransaction,tblCustomer objCust)
        {
            bool Result = true;
            try
            {
                tblCustomer customer = new tblCustomer();
                if (objCust.Id==0)
                {
                    Result = false;
                    using (DPTContext context = new DPTContext())
                    {                       
                        customer.CustomerName = objCust.CustomerName;
                        customer.MobileNo = objCust.MobileNo;
                        customer.CreatedBy = 2;
                        customer.CreatedDate = DateTime.Now;
                        context.tblCustomers.Add(customer);
                        context.SaveChanges();
                        objTransaction.CustomerId = customer.Id;
                        Result = true;
                    }
                }
                if (Result)
                {
                   
                    objTransaction.InvoiceDate = DateTime.Now;
                    objTransaction.CreatedBy = 2;
                    objTransaction.CreatedDate = DateTime.Now;

                    
                    using (DPTContext context = new DPTContext())
                    {
                        context.tblTransactions.Add(objTransaction);
                        context.SaveChanges();
                        context.tblTransactionItems.Add(objItem);
                        context.SaveChanges();
                        tblStock stock=context.tblStocks.Where(x => x.Id == objItem.ProductId).SingleOrDefault();
                        stock.ProductQty -= Convert.ToInt32(objItem.Qty);
                        context.tblStocks.AddOrUpdate(stock);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
    
            return Result;
        }

        public List<tblTransactionItem> GetTransactionItemsByInvoiceNo(string invoiceNo)
        {
            List<tblTransactionItem> lstItem = new List<tblTransactionItem>();
            using (DPTContext context = new DPTContext())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                lstItem = context.tblTransactionItems.Where(x => x.InvoiceId == invoiceNo).ToList();
                foreach(tblTransactionItem item in lstItem)
                {
                    item.tblStock = context.tblStocks.Where(y => y.Id == item.ProductId).FirstOrDefault();
                }
            }
            return lstItem;
        }

        public tblTransaction GetTransactionByInvoiceNo(string invoiceNo)
        {
            tblTransaction objData = new tblTransaction();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                objData = context.tblTransactions.Where(x => x.InvoiceNo == invoiceNo).FirstOrDefault();
                
            }
            return objData;
        }
    }
}
