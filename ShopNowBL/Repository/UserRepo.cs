using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using ShopNowBL.Models;

namespace ShopNowBL.Repository
{
    public class UserRepo
    {
        public List<tblUser> GetAllUsers()
        {

            List<tblUser> lstUsers = new List<tblUser>();
            using (DPTContext context = new DPTContext())
            {

                lstUsers = context.tblUsers.ToList();
                return lstUsers;
            }

        }

        public List<tblUser> GetAllAdmin()
        {

            List<tblUser> lstAdmin = new List<tblUser>();
            using (DPTContext context = new DPTContext())
            {

                lstAdmin = context.tblUsers.Where(x => x.RoleId == 2).ToList();
                 return lstAdmin;
            }

        }
        //Add Admin
        public bool AddAdmin(tblUser user)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                try
                {
                    user.RoleId = 2;
                    user.CreatedDate = DateTime.Now;
                    user.CreatedBy = 1;
                    context.tblUsers.AddOrUpdate(user);
                    context.SaveChanges();
                    Result = true;
                }

                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return Result;
        }

        //Admin Login
        public tblUser ValidateUser(string emailId, string password)
        {
            tblUser objUser = new tblUser();
            using (DPTContext context = new DPTContext())
            {
                objUser = context.tblUsers.Where(x => x.EmailId == emailId && x.Password == password).FirstOrDefault();
            }
            return objUser;
        }

        public List<tblUser> GetAllCashiers()
        {

            List<tblUser> lstCashiers = new List<tblUser>();
            using (DPTContext context = new DPTContext())
            {
                try
                {
                    /*lstCashiers = (from tblUser in context.tblUsers
                                    where tblUser.RoleId == 3 
                                    select tblUser).ToList();*/
                    lstCashiers = context.tblUsers.Where(x => x.RoleId == 3).ToList();

                    return lstCashiers;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

        }

        public bool AddCashier(tblUser user)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                user.RoleId = 3;
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = 2;
                context.tblUsers.AddOrUpdate(user);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }
        
        public bool DeleteUserById(int id)
        {
            bool result = false;
            using (DPTContext context = new DPTContext())
            {
                var objCashier = context.tblUsers.SingleOrDefault(x => x.Id == id);
                context.tblUsers.Remove(objCashier);
                context.SaveChanges();
                result = true;
            }
                
            return result;
        }

        public tblUser GetUserDetails(int id)
        {
            tblUser objCashier = new tblUser();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                objCashier = context.tblUsers.SingleOrDefault(x => x.Id == id);
                
            }

            return objCashier;
        }

        public bool AddUser(tblUser user)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
               // user.RoleId = 2;
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = 2;
                context.tblUsers.AddOrUpdate(user);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }
    }
}
