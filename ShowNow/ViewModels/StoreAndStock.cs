using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNowBL.Models;

namespace ShowNow.ViewModels
{
    public class StoreAndStock
    {
        public List<tblStore> lstStores { get; set; }

        public List<tblStock> lstStocks { get; set; }
    }
}