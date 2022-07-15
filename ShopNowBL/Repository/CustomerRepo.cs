using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNowBL.Models;
using System.Data.Entity.Migrations;

namespace ShopNowBL.Repository
{
    public class CustomerRepo
    {

        //Select all customers
        public List<tblCustomer> getAllCustomers()
        {
            List<tblCustomer> lstCustomer = new List<tblCustomer>();
            using (DPTContext context = new DPTContext())
            {
                lstCustomer = context.tblCustomers.ToList();
            }
            return lstCustomer;
        }

        //Add customer
        public bool addCustomer(tblCustomer customer)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                customer.CreatedDate = DateTime.Now;
                customer.CreatedBy = 3;
                context.tblCustomers.AddOrUpdate(customer);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }

        public bool DeleteCustomerById(int id)
        {
            bool result = false;
            tblCustomer objCustomer = new tblCustomer();
            using (DPTContext context = new DPTContext())
            {
                objCustomer = context.tblCustomers.SingleOrDefault(x => x.Id == id);
                context.tblCustomers.Remove(objCustomer);
                context.SaveChanges();
                result = true;
            }

            return result;
        }

        //update
        public tblCustomer getCustomerById(int id)
        {
            tblCustomer objCustomer = new tblCustomer();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                objCustomer = context.tblCustomers.Where(x => x.Id == id).FirstOrDefault();

            }

            return objCustomer;
        }

        public tblCustomer getCustomerByMobileNo(string MobileNo)
        {
            tblCustomer objCustomer = new tblCustomer();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                objCustomer = context.tblCustomers.Where(x => x.MobileNo == MobileNo).FirstOrDefault();

            }

            return objCustomer;
        }
    }
}
