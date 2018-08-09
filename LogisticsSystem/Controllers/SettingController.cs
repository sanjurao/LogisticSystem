using Logistic.Base.DataContext;
using Logistic.Base.Entity;
using LogisticsSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using LogisticsSystem.Controllers;
using LogisticSystem.Models;

namespace LogisticSystem.Controllers
{
    [Authorize]
    public class SettingController : BaseController
    {
        //
        // GET: /Setting/
        Logistic.Base.DataContext.LogisticsConnection db = new Logistic.Base.DataContext.LogisticsConnection();
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult RoleSettings()
        {
            return View();

        }
        public ActionResult SaveUser(string UserType)
        {
            var data = db.tblUserType.Where(x => x.Name == UserType).Count();
            if (data > 0)
            {
                TempData["Msg"] = "" + UserType + "" + " " + " Is already exists. ";
            }
            else
            {
                tblUserType type = new tblUserType();
                type.Name = UserType;
                type.Status = false;
                db.tblUserType.Add(type);
                db.SaveChanges();
                ModelState.Clear();
                TempData["Msg"] = "Saved Successfully";
            }
            return RedirectToAction("RoleSettings");

        }
        public ActionResult SaveMenues(string Menues, string MenueLink, string Submenu, string SubmenueLink)
        {
            var data = db.tblmenues.Where(x => x.Name == Menues).Count();
            var submenuedata = db.tblSubemenues.Where(x => x.Submenue == Submenu).Count();
            if (data > 0)
            {
                TempData["Msg"] = "" + Menues + "" + " " + " Is already exists. ";
            }
            else
            {
                tblmenues type = new tblmenues();
                type.Name = Menues;
                type.Status = false;
                type.MenuLink = MenueLink;
                db.tblmenues.Add(type);
                db.SaveChanges();                   
                ModelState.Clear();
            }
            int menueid = db.tblmenues.Where(x => x.Name == Menues).Select(x => x.Id).FirstOrDefault();
            if (submenuedata > 0)
            {
                TempData["Msg"] = "" + Menues + "" + " " + " Is already exists. ";
            }
            else
            {
                if (Submenu != null)
                {

                    tblSubemenues type = new tblSubemenues();
                    type.Submenue = Submenu;
                    type.MenueId = menueid;
                    type.SubMenueLink = SubmenueLink;
                    db.tblSubemenues.Add(type);
                    db.SaveChanges();
                    ModelState.Clear();
                }
            }
            TempData["Msg"] = "Saved Successfully";
            return RedirectToAction("RoleSettings");

        }
        public ActionResult SaveMenueForUser(MenueUser model)
        {
            var data = db.tblRole.Where(x => x.menuId == model.MenueId && x.typeId == model.UserId).Count();
            if (data > 0)
            {
                TempData["Msg"] = "" + model.MenueId + " " + " " + " is Added For " + " " + " " + model.UserId + "";
            }
            else
            {
                tblRole role = new tblRole();
                role.menuId = model.MenueId;
                role.typeId = model.UserId;
                role.Status = false;
                db.tblRole.Add(role);
                db.SaveChanges();
                TempData["Msg"] = "Saved Successfully";
            }
            return RedirectToAction("RoleSettings");
        }
        public ActionResult ThemeDetails()
        {
            return View();
        }
        public ActionResult ItemDetails()
        {
            return View();
        }
        public ActionResult InputTheme(int? id, string E)
        {
            if (id > 0 && E == "D")
            {
                tblTheme model = db.tblTheme.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";    
                return RedirectToAction("ThemeDetails");

            }

            else if (id > 0)
            {
                tblTheme model = db.tblTheme.Find(id);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult InputTheme(tblTheme model, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var cm = db.tblTheme.Where(x => x.ID == id).FirstOrDefault();
                    cm.ID = Convert.ToInt32(id);
                    cm.Name = model.Name;
                    db.Entry(cm).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Update Succussfully";
                    return RedirectToAction("ThemeDetails");
                }
                else
                {
                    model.Name = model.Name;

                    model.Status = true;
                    db.tblTheme.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = " Created Successfully";
                    return RedirectToAction("ThemeDetails");
                }
            }
            return View();
        }
        public ActionResult InputItem(int? id, string E)
        {
            if (id > 0 && E == "D")
            {
                tblItem model = db.tblItem.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";    
                return RedirectToAction("ItemDetails");

            }

            else if (id > 0)
            {
                tblItem model = db.tblItem.Find(id);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult InputItem(tblItem model, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var cm = db.tblItem.Where(x => x.Id == id).FirstOrDefault();
                    cm.Id = Convert.ToInt32(id);
                    cm.ItemName = model.ItemName;
                    cm.ItemDesc = model.ItemDesc;
                    cm.UOM = model.UOM;
                    db.Entry(cm).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Update Succussfully";
                    return RedirectToAction("ItemDetails");
                }
                else
                {
                    model.ItemName = model.ItemName;
                    model.ItemDesc = model.ItemDesc;
                    model.UOM = model.UOM;
                    model.Status = true;
                    db.tblItem.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = " Created Successfully";
                    return RedirectToAction("ItemDetails");
                }
            }
            return View();
        }
        public ActionResult MOTDetails()
        {
            return View();
        }
        public ActionResult InputMOTDetails(int? id, string E)
        {
            if (id > 0 && E == "D")
            {
                tblMOT model = db.tblMOT.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";    
                return RedirectToAction("MOTDetails");

            }

            else if (id > 0)
            {
                tblMOT model = db.tblMOT.Find(id);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult InputMOTDetails(tblMOT model, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var cm = db.tblMOT.Where(x => x.ID == id).FirstOrDefault();
                    cm.ID = Convert.ToInt32(id);
                    cm.Name = model.Name;
                    db.Entry(cm).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Update Succussfully";
                    return RedirectToAction("MOTDetails");
                }
                else
                {
                    model.Status = true;
                    db.tblMOT.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Created Sucessfully!";                  
                    return RedirectToAction("MOTDetails");
                }
            }
            return View();
        }
        public ActionResult KPIDetails()
        {
            return View();
        }
        public ActionResult InputKPI(int? id, string E)
        {
            if (id > 0 && E == "D")
            {
                tblKPI model = db.tblKPI.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";
                return RedirectToAction("KPIDetails");

            }

            else if (id > 0)
            {
                tblTheme model = db.tblTheme.Find(id);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult InputKPI(tblKPI model, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var cm = db.tblKPI.Where(x => x.Id == id).FirstOrDefault();
                    cm.Id = Convert.ToInt32(id);
                    cm.KPI = model.KPI;
                    db.Entry(cm).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Update Succussfully";
                    return RedirectToAction("KPIDetails");
                }
                else
                {
                    model.KPI = model.KPI;
                    db.tblKPI.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = " Created Successfully";
                    return RedirectToAction("KPIDetails");
                }
            }
            return View();
        }
        public ActionResult ChargeCodeDetails()
        {
            return View();
        }
        public ActionResult InputChargeCode(int? id, string E)
        {
            var Originator = (from x in db.Users.Where(x => x.UserType == "Originator") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.Originator = Originator;
            if (id > 0 && E == "D")
            {
                PM_budgettable model = db.PM_budgettable.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";         
                return RedirectToAction("ChargeCodeDetails");
            }

            else if (id > 0)
            {
                PM_budgettable model = db.PM_budgettable.Find(id);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult InputChargeCode(PM_budgettable model, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var cm = db.PM_budgettable.Where(x => x.Chargecode == id).FirstOrDefault();
                    cm.Chargecode = Convert.ToInt32(id);
                    cm.Chargecodename = model.Chargecodename;
                    db.Entry(cm).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Update Succussfully";
                    return RedirectToAction("ChargeCodeDetails");
                }
                else
                {
                    db.PM_budgettable.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = " Created Successfully";
                    return RedirectToAction("ChargeCodeDetails");
                }
            }
            return View();
        }
        public ActionResult CategoryDetails()
        {
            return View();
        }
        public ActionResult Category(int? id, string E)
        {
            if (id > 0 && E == "D")
            {
                CM_Categories model = db.CM_Categories.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";        
                return RedirectToAction("CategoryDetails");
            }

            else if (id > 0)
            {
                CM_Categories model = db.CM_Categories.Find(id);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Category(CM_Categories model, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Update Succussfully";
                }
                else
                {
                    db.CM_Categories.Add(model);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["msg"] = "Category Created Successfully";
                }
            }
            return RedirectToAction("CategoryDetails");
        }

        public ActionResult ApprovalTimeDetails()
        {
            return View();
        }
        public ActionResult ApprovalTime(int? id, string E)
        {
            if (id > 0 && E == "D")
            {
                tblApprovalTime model = db.tblApprovalTime.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";  
                return RedirectToAction("ApprovalTimeDetails");
            }

            else if (id > 0)
            {
                tblApprovalTime model = db.tblApprovalTime.Find(id);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ApprovalTime(tblApprovalTime model, int? id)
        {

            if (id > 0)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();
                TempData["msg"] = "Update Succussfully";
            }
            else
            {

                db.tblApprovalTime.Add(model);
                db.SaveChanges();
                ModelState.Clear();
                TempData["msg"] = "Category Created Successfully";
            }
            return RedirectToAction("ApprovalTimeDetails");
        }

        public ActionResult LeadTimeDetails()
        {
            return View();
        }
        public ActionResult LeadTime(int? id, string E)
        {

            if (id > 0 && E == "D")
            {
                tblSupplier model = db.tblSupplier.Find(id);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                TempData["msg"] = "Deleted Sucessfully!";  
                return RedirectToAction("LeadTimeDetails");
            }

            else if (id > 0)
            {
                tblSupplier model = db.tblSupplier.Find(id);
                return View(model);
            }
            else
            {
                var Supplier = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Supplier") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
                ViewBag.supplierName = Supplier;
                return View();
            }

        }
        [HttpPost]
        public ActionResult LeadTime(int? Id, tblSupplier model)
        {
            if (Id > 0)
            {
                //var supplierid=UnitOfWork.UserRepository.Get(x=>x.Id==model.
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["msg"] = "Updated Sucessfully!";  
                ModelState.Clear();
            }
            else
            {
                tblSupplier supplier = new tblSupplier();
                supplier.SupplierID = Convert.ToInt64(model.SupplierName);
                int id = Convert.ToInt32(model.SupplierName);
                var Name = db.Users.Where(x => x.Id == id).Select(x => x.FirstName).FirstOrDefault();
                supplier.SupplierName = Name.ToString();
                supplier.LeadTime = model.LeadTime;
                db.tblSupplier.Add(supplier);
                db.SaveChanges();
                TempData["msg"] = "Added Sucessfully!";  
                ModelState.Clear();
            }
            return RedirectToAction("LeadTimeDetails");
        }
    }
}
