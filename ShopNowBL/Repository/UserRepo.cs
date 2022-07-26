using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using ShopNowBL.Models;
using System.Security.Cryptography;
using System.IO;

namespace ShopNowBL.Repository
{
    public class UserRepo
    {
        //save user
        public bool AddUser(tblUser user)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                context.tblUsers.AddOrUpdate(user);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }

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

        public List<tblUser> GetAllCashiers()
        {

            List<tblUser> lstCashiers = new List<tblUser>();
            using (DPTContext context = new DPTContext())
            {
                try
                {
                    lstCashiers = context.tblUsers.Where(x => x.RoleId == 3).ToList();

                    return lstCashiers;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

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

        //get user by email
        public tblUser VerifyEmail(string email)
        {
            tblUser objUser = new tblUser();
            using (DPTContext context = new DPTContext())
            {
                objUser = context.tblUsers.Where(x => x.EmailId == email).FirstOrDefault();
            }
            return objUser;
        }

        //add otp obj to db
        public bool AddOtpToDb(tblOTP objOtp)
        {
            bool result = false;
            using (DPTContext context = new DPTContext())
            {
                context.tblOTPs.AddOrUpdate(objOtp);
                context.SaveChanges();
                result = true;
            }

            return result;
            
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

        public tblOTP GetObjOtpByEmail(string email)
        {
            tblOTP objOtp = new tblOTP();
            using (DPTContext context = new DPTContext())
            {
                objOtp = context.tblOTPs.Where(x => x.EmailId == email && x.IsUsed == 0)
                    .OrderByDescending(t => t.Created_DateTime).FirstOrDefault();
            }
            return objOtp;
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

        //******************Encrypt Password********************************** 
        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
