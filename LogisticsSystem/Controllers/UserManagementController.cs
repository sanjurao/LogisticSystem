using System.Globalization;
using IdentitySample.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Logistic.Base.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using LogisticsSystem.Controllers;
using System.Net;
using LogisticSystem.Models;
using Logistic.Base.DataContext;
using System.Collections.Generic;

namespace LogisticSystem.Controllers
{
    [Authorize]
    public class UserManagementController : BaseController
    {
        //
        // GET: /UserManagement/
        public UserManagementController()
        { }
        public UserManagementController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetUser(int? Id, string E, string D)
        {
            LogisticsConnection ctx = new LogisticsConnection();
            if (Id > 0 && E == "A")
            {
                var status = UnitOfWork.UserRepository.Get(x => x.Id == Id).IsActive;
                if (status == false)
                {
                    var studentList = ctx.Database.ExecuteSqlCommand("Update AspNetUsers set IsActive=1 where Id='" + Id + "'");
                    List<UM_User> user = UnitOfWork.UserRepository.GetAll().ToList();
                    return View(user);
                }
                else
                {
                    var studentList = ctx.Database.ExecuteSqlCommand("Update AspNetUsers set IsActive=0 where Id='" + Id + "'");
                    List<UM_User> user = UnitOfWork.UserRepository.GetAll().ToList();
                    return View(user);
                }
            }
            else if (Id > 0 && D == "R")
            {
                var studentList = ctx.Database.ExecuteSqlCommand("delete from AspNetUsers where Id='" + Id + "'");
                List<UM_User> user = UnitOfWork.UserRepository.GetAll().ToList();
                return View(user);
            }
            else
            {
                List<UM_User> user = UnitOfWork.UserRepository.GetAll().ToList();
                return View(user);
            }
        }
        [HttpGet]
        public ActionResult Createuser()
        {
            var countryList = new SelectList(Country.CountryList());
            ViewBag.country = countryList;
            return View();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Createuser(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var user = new UM_User() { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Password = model.Password, Image = model.Image.FileName, TimeZone = model.TimeZone, StartPage = model.StartPage, FeedSelection = model.FeedSelection, Gender = model.Gender, Address = model.Address, City = model.City, ZipCode = model.ZipCode, IsActive = false, Country = model.Country, UserType = model.UserType };
                    UM_User user = new UM_User();
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Password = model.Password;
                    //user.Image = model.Image.FileName;
                    user.TimeZone = model.TimeZone;
                    user.StartPage = model.StartPage;
                    user.FeedSelection = model.FeedSelection;
                    user.Gender = model.Gender;
                    user.IsEmailNotificationActive = model.IsEmailNotificationActive;
                    user.Address = model.Address;
                    user.City = model.City;
                    user.ZipCode = model.ZipCode;
                    user.IsActive = false;
                    user.Country = model.Country;
                    user.UserType = model.UserType;
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        Response.Write("<script>alert('Regester Successfully')</script>");
                        ModelState.Clear();
                    }
                    AddErrors(result);
                }
                var countryList = new SelectList(Country.CountryList());
                ViewBag.country = countryList;
            }
            catch { }
            return View();
        }
    }
}
