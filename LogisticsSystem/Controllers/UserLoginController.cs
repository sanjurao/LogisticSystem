using Logistic.Base.DataContext;
using Logistic.Base.Entity;
using LogisticsSystem.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LogisticsSystem.Controllers
{
    public class UserLoginController : BaseController
    {
        const string password = "password";
        LogisticsConnection db = new LogisticsConnection();
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult UserLogin()
        {
            AccountModel user = new AccountModel();
            if (Request.Cookies["username"] != null && Request.Cookies["pwd"] != null)
            {

                string username = DecryptData(Request.Cookies["username"].Value);
                string password = DecryptData(Request.Cookies["pwd"].Value);
                user.login.UserName = username;
                user.login.Password = password;
            }

            return View(user);
        }
        public static string EncryptData(string str)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(password));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8.GetBytes(str);
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return Convert.ToBase64String(Results);
        }
        public static string DecryptData(string str)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(password));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(str);
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return UTF8.GetString(Results);
        }
        [HttpPost]
        public ActionResult UserLogin(AccountModel model, IEnumerable<bool> chkbox)
        {
            var enyusername = EncryptData(model.login.UserName);
            var enypwd = EncryptData(model.login.Password);
            HttpCookie user = new HttpCookie("username", enyusername);
            HttpCookie pwd = new HttpCookie("pwd", enypwd);
            if (chkbox != null && chkbox.Count() == 2)
            {
                this.Response.Cookies.Add(user);
                this.Response.Cookies.Add(pwd);
                user.Expires.AddDays(30);
                pwd.Expires.AddDays(30);
            }
            else
            {
                user.Expires = DateTime.Now.AddDays(-1);
                user.Value = null;
                Response.Cookies.Add(user);
                pwd.Expires = DateTime.Now.AddDays(-1);
                pwd.Value = null;
                Response.Cookies.Add(pwd);
            }
            try
            {
                var logindetails = UnitOfWork.UserRepository.Get(x => x.UserName.Equals(model.login.UserName) && x.Password.Equals(model.login.Password));
                string usertype = UnitOfWork.UserRepository.Get(x => x.UserName == model.login.UserName).UserType;
                bool isactive = UnitOfWork.UserRepository.Get(x => x.UserName == model.login.UserName).IsActive;

                if (logindetails != null)
                {
                    if (logindetails.UserType == "SuperAdmin")
                    {
                        Session.Add("UserName", model.login.UserName);
                        Response.Redirect("~/UserLogin/SuperAdmin");

                    }
                    else if (logindetails.UserType == "Admin")
                    {
                        //Session["UserName"] = model.login.UserName;
                        Session.Add("UserName", model.login.UserName);
                        Response.Redirect("~/UserLogin/SuperAdmin");
                    }
                    else if (logindetails.UserType == "Requitioner" && isactive == Convert.ToBoolean(1))
                    {
                        Session.Add("UserName", model.login.UserName);
                        Response.Redirect("~/UserLogin/SuperAdmin");
                    }
                    else if (logindetails.UserType == "Consolidatar" && isactive == true)
                    {
                        Session.Add("UserName", model.login.UserName);
                        Response.Redirect("~/UserLogin/SuperAdmin");
                    }
                    else if (logindetails.UserType == "End User" && isactive == true)
                    {
                        Session.Add("UserName", model.login.UserName);
                        Response.Redirect("~/UserLogin/SuperAdmin");
                    }
                    else if (logindetails.UserType == "Originatar" && isactive == true)
                    {
                        Session.Add("UserName", model.login.UserName);
                        Response.Redirect("~/UserLogin/SuperAdmin");
                    }
                    else
                    {
                        Response.Write("<script>alert('You are not register now.')</script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('User Name Or Password is Invalid')</script>");
                }


            }
            catch { Response.Write("<script>alert('User Name Or Password is Invalid')</script>"); }

            return View();
        }

        [HttpPost]
        public ActionResult UserReg(AccountModel um)
        {
            UM_User user = new UM_User();
            var emailid = UnitOfWork.UserRepository.Get(x => x.Email.Equals(um.UserReg.Email));
            UM_User user1 = UnitOfWork.UserRepository.Get(x => x.UserName.Equals(um.UserReg.UserName));
            //if (user1 != null)
            //{
            //    if(user1.UserName==um.UserReg.UserName)
            //}
            if (emailid == null && user1 == null)
            {
                if (um.UserReg.Password != um.UserReg.ReTypePwd)
                {
                    Response.Write("<script>alert('password and retype password not matched')</script>");
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        foreach (ModelState modelState in ModelState.Values)
                        {
                            foreach (ModelError error in modelState.Errors)
                            {
                                Response.Write("<script>alert('Please Fill All Filds.')</script>");
                            }
                        }

                    }
                    else
                    {
                        user.FirstName = um.UserReg.FirstName;
                        user.Email = um.UserReg.Email;
                        user.Address = um.UserReg.Address;
                        user.Country = um.UserReg.Country;
                        user.UserName = um.UserReg.UserName;
                        user.Password = um.UserReg.Password;
                        user.City = um.UserReg.City;
                        user.UserType = um.UserReg.UserType;
                        user.IsActive = false;
                        user.Password = um.UserReg.PasswordSalt = EncryptData(um.UserReg.Password);
                        UnitOfWork.UserRepository.Insert(user);
                        AM_GlobalRole role = new AM_GlobalRole();
                        role.Caption = um.UserReg.UserType;
                        role.Description = um.UserReg.UserType;
                        UnitOfWork.GlobalRoleRepository.Insert(role);
                        Response.Write("<script>alert('Register Successfully')</script>");
                    }
                }

            }
            else
            {
                Response.Write("<script>alert('This username already exist. Please try with another username.')</script>");
            }

            return View("UserLogin");
        }
        [HttpGet]
        public ActionResult SuperAdmin()
        {
            ViewBag.User = UnitOfWork.UserRepository.GetAll().ToList();
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(UserViewModel Email)
        {
            var email = Request["Email"];

            return View("UserLogin");
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            Session.Clear();

            return View("UserLogin");
        }
        public ActionResult Approve(int id)
        {
            int noOfRowDeleted = db.Database.ExecuteSqlCommand("update UM_User set isActive=1 where ID='" + id + "'");
            return RedirectToAction("SuperAdmin", "UserLogin", null);
        }
    }
}
