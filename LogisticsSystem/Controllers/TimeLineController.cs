using Logistic.Base.DataContext;
using Logistic.Base.Entity;
using LogisticsSystem.Controllers;
using LogisticSystem.SendMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogisticSystem.Controllers
{
    public class TimeLineController : BaseController
    {
        //
        // GET: /TimeLine/
        LogisticsConnection db = new LogisticsConnection();
        MailSending send = new MailSending();

        private void Sendmail(int id, string subject, string body)
        {
            string To = UnitOfWork.UserRepository.Get(x => x.Id == id).Email;
            bool SendEmailNotification = Convert.ToBoolean(db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.IsEmailNotificationActive).FirstOrDefault());
            if (SendEmailNotification)
            send.sendMail(To, subject, body);
        }
        //to send emails.
        public void SendtimelineEmail(string PONO)
        {
            int pono = PONO.Length;
            string po = PONO.Substring(1, pono - 2);
            var users = UnitOfWork.OrderRepository.GetAll().Where(x => x.MSD_PO == po).FirstOrDefault();
            int AdminId = UnitOfWork.UserRepository.Get(x => x.UserType == "Admin").Id;
            string commentdata = Session["commentdata"].ToString();
            if (users.EndUserId > 0)
            {
                Sendmail(users.EndUserId, "Timeline Notification", commentdata);
            }
            if (users.OrgUID > 0)
            {
                int Id = Convert.ToInt32(users.OrgUID);
                Sendmail(Id, "Timeline Notification", commentdata);
            }
            if (users.ReqUID > 0)
            {
                int Id = Convert.ToInt32(users.ReqUID);
                Sendmail(Id, "Timeline Notification", commentdata);
            }
            if (users.SupplierID > 0)
            {
                Sendmail(users.SupplierID, "Timeline Notification", commentdata);
            }
            if (users.Consolidator > 0)
            {
                int Id = Convert.ToInt32(users.Consolidator);
                Sendmail(Id, "Timeline Notification", commentdata);
            }
            if (AdminId > 0)
            {
                Sendmail(AdminId, "Timeline Notification", commentdata);
            }
        }
        [HttpGet]
        public ActionResult TimeLine(string Id,string MSD_PO)
        {
            if (MSD_PO != null)
            {
                TempData["MsdPo"] = MSD_PO;
                TempData["Id"] = Id;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Comment(int? id, tblcomment model, string OrderId, string url, int TimelineId)
        {
            if (id > 0)
            {
                var ID = db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
                model.commentdata = model.commentdata;
                model.UserId = Convert.ToInt32(id);
                model.commentdate = System.DateTime.Now.ToString();
                model.CommentBy = ID.ToString();
                model.CommentOn = OrderId;
                model.TimelineId = TimelineId;
                db.tblcomment.Add(model);
                Session["commentdata"] = "<table style=color:green><tr><td colspan=3>Logistic System</td><td></td></tr><tr><td><ul><li>User Name</li><li>Status</li><li>PO Number</li></ul></td><td><ul><li>" + User.Identity.Name + "</li><li>" + model.commentdata + "</li><li>" + OrderId + "</li></ul></td></tr></table>";
                db.SaveChanges();
                ModelState.Clear();
                TempData["run"] = "true";
            }
            TempData["MsdPo"] = OrderId;
            return Redirect(url);
        }


    }
}
