using Logistic.Base.DataContext;
using Logistic.Base.Entity;
using LogisticSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogisticSystem.Controllers
{
    public class RoleManagementController : Controller
    {
        //
        // GET: /RoleManagement/
        LogisticsConnection db = new LogisticsConnection();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageRole()
        {
            try
            {
                RoleViewModel model = new RoleViewModel
                {
                    usertype = db.tblUserType.ToList(),
                    menues = db.tblmenues.ToList(),
                    Role = db.tblRole.ToList()
                };
                return View(model);
            }
            catch { return View(); }

        }

        [HttpPost]
        public ActionResult ManageRole(int[] uid, int[] sid)
        {
            try
            {
                string btnupdate = Request.Form["btnupdate"];
                if (btnupdate == "DisAllow")
                {
                    for (int i = 0; i < sid.Length; i++)
                    {
                        int UserId = Convert.ToInt32(uid[0]);
                        int Id = Convert.ToInt32(sid[i]);
                        var Roll = db.tblRole.Where(x => x.typeId == UserId && x.menuId == Id).ToList();
                        if (Roll.Count > 0)
                        {
                            var RollUpdate = db.tblRole.Where(x => x.typeId == UserId && x.menuId == Id).FirstOrDefault();
                            RollUpdate.Status = false;
                            db.Entry(RollUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                    }
                }
                else
                {
                    for (int i = 0; i < sid.Length; i++)
                    {
                        int UserId = Convert.ToInt32(uid[0]);
                        int Id = Convert.ToInt32(sid[i]);
                        var RollTable = db.tblRole.Where(x => x.typeId == UserId && x.menuId == Id).ToList();
                        if (RollTable.Count > 0)
                        {
                            var RollTable1 = db.tblRole.Where(x => x.typeId == UserId && x.menuId == Id).FirstOrDefault();
                            RollTable1.Status = true;
                            db.Entry(RollTable1).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                    }
                }
                RoleViewModel model = new RoleViewModel
                {
                    usertype = db.tblUserType.ToList(),
                    menues = db.tblmenues.ToList(),
                    Role = db.tblRole.ToList()
                };
                return View(model);
            }
            catch { return View(); }

        }
    }
}
