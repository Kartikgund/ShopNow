using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNowBL.Models;

namespace ShowNow.ViewModels
{
    public class UserAndCustomer
    {
        public List<tblCustomer> lstCustomers { get; set; }

        public List<tblUser> lstUsers { get; set; }

        public int customerCount { get; set; }

        public int userCount { get; set; }
    }
}