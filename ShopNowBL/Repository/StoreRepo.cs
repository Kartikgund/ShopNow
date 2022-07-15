using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using ShopNowBL.Models;

namespace ShopNowBL.Repository
{
    public class StoreRepo
    {

        public List<tblStore> GetAllStores()
        {
            List<tblStore> lstStores = new List<tblStore>();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                lstStores = context.tblStores.ToList();
            }

            return lstStores;
        }

        public bool SaveStore(tblStore store)
        {
            bool result = false;
            using (DPTContext context = new DPTContext())
            {
                store.CreatedDate = DateTime.Now;
                store.CreatedBy = 2;
                store.StartedDate = DateTime.Now;
                context.tblStores.AddOrUpdate(store);
                context.SaveChanges();
                result = true;
            }

            return result;
        }

        public tblStore FindStoreById(int id)
        {
            tblStore objStore = new tblStore();
            using (DPTContext context = new DPTContext())
            {
                objStore = context.tblStores.Where(x => x.Id == id).FirstOrDefault();

            }
                return objStore;
        }

        public bool DeleteStoreById(int id)
        {
            bool result = false;
            tblStore objStore = new tblStore();
            using (DPTContext context = new DPTContext())
            {
                objStore = context.tblStores.Where(x => x.Id == id).FirstOrDefault();
                context.tblStores.Remove(objStore);
                context.SaveChanges();
                result = true;
            }
            return result;
        }
    }
}
