using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNowBL.Models;
using ShopNowBL.Repository;

namespace ShowNow.ViewModels
{
    public class UserAndStore
    {
        public tblUser objUser { get; set; }

        public List<tblStore> lstStore { get; set; }
    }
}