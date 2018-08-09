using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logistic.Base.Entity;
using Microsoft.AspNet.Identity;
using Logistic.Base.DataContext;
using System.Data.Entity;
using LogisticsSystem.Models;

namespace LogisticsSystem.Controllers
{
    [Authorize]
    public class UserProfileController : BaseController
    {
        //
        // GET: /UserProfile/
        Logistic.Base.DataContext.LogisticsConnection db = new Logistic.Base.DataContext.LogisticsConnection();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyProfile()
        {
            UM_User userdetails = null;
            try
            {
                if (User.Identity.Name != null)
                {
                    string name = User.Identity.Name;
                    userdetails = UnitOfWork.UserRepository.GetAll().Where(x => x.Email == name).FirstOrDefault();
                }
            }
            catch { }
            return View(userdetails);
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            int id = Convert.ToInt32(User.Identity.GetUserId());
            UM_User Userdetails = UnitOfWork.UserRepository.GetAll().Where(x => x.Id == id).FirstOrDefault();
            UserViewModel model = new UserViewModel();
            model.FirstName = Userdetails.FirstName;
            model.LastName = Userdetails.LastName;
            model.Gender = Userdetails.Gender;
            model.City = Userdetails.City;
            model.Country = Userdetails.Country;
            model.PhoneNumber = Userdetails.PhoneNumber;
            model.ZipCode = Userdetails.ZipCode;
            model.Address = Userdetails.Address;
            return View(model);
        }
        [HttpPost]
        public ActionResult EditProfile(UserViewModel user, HttpPostedFileBase Image)
        {
            int id = Convert.ToInt32(User.Identity.GetUserId());
           
            UM_User Userdetails = db.Users.Where(x => x.Id == id).FirstOrDefault();
            if (Image != null)
            {
                if (Image.ContentLength > 300)
                {
                    string fileName = Image.FileName;
                    Image.SaveAs(Server.MapPath("~/Images/" + fileName));

                    Userdetails.Image = fileName;
                }
            }
            Userdetails.FirstName = user.FirstName;
            Userdetails.LastName = user.LastName;
            Userdetails.Gender = user.Gender;
            Userdetails.PhoneNumber = user.PhoneNumber;
            Userdetails.Address = user.Address;
            Userdetails.City = user.City;
            Userdetails.ZipCode = user.ZipCode;
            Userdetails.Country = user.Country;
           // Userdetails.Image = user.Image.FileName;
            db.Entry(Userdetails).State = EntityState.Modified;  //to update
            db.SaveChanges();
            return RedirectToAction("MyProfile");
        }
        public ActionResult LockScreen()
        {
            return View();
        }
        public ActionResult MyInbox()
        {
            return View();
        }
    }
}
