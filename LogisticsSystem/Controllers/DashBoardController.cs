using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logistic.Base.DataContext;
using Logistic.Base.Entity;
using LogisticsSystem.Controllers;
using System.Web.Helpers;
using System.Globalization;
using LogisticSystem.SendMail;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
namespace LogisticSystem.Controllers
{
    public class DashBoardController : BaseController
    {
        //
        // GET: /DashBoard/
        LogisticsConnection db = new LogisticsConnection();
        MailSending mail = new MailSending();
        private void prndingOrder()
        {
            var date = DateTime.UtcNow.ToString("MM/dd/yyyy");
            var date3 = db.PM_Order.ToList();
            foreach (var k in date3)
            {
                var test = k.CreatedDate;
                var tesdnkdnk = db.PM_Order.Where(x => x.CreatedDate == test).ToList();
            }
            var item = db.CM_User_Notification.Where(x => x.PendingDate == date && x.PendingOrderStatus == false).ToList();
            if (item.Count > 0)
            {
                foreach (var i in item)
                {
                    CM_User_Notification model = new CM_User_Notification();
                    var PoNo = db.PM_Order.Where(x => x.ID == i.orderID).Select(x => x.ORAPoNumber).FirstOrDefault();
                    model.EntityID = i.EntityID;
                    model.ActorID = i.ActorID;
                    model.AttributeName = i.AttributeName;
                    model.CreatedOn = i.CreatedOn;
                    model.FromValue = i.FromValue;
                    model.IsSentInMail = i.IsSentInMail;
                    model.IsViewed = i.IsViewed;
                    model.orderStatus = i.orderStatus;
                    model.PendingDate = i.PendingDate;
                    model.PendingOrderStatus = i.PendingOrderStatus;
                    model.ToValue = i.ToValue;
                    model.TypeID = i.TypeID;
                    model.TypeName = i.TypeName;
                    model.UserID = i.UserID;
                    model.orderID = model.orderID;
                    model.Content = PoNo + "" + "This Order Is Pending From Supplier";
                    db.CM_User_Notification.Add(model);
                    db.SaveChanges();
                }
            }
        }

        private bool SendNotification(long orderid, int ToValue)
        {
            string uname = (string)User.Identity.Name;
            bool SendEmailNotification = Convert.ToBoolean(db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.IsEmailNotificationActive).FirstOrDefault());
            var item = db.PM_Order.Where(x => x.ID == orderid).FirstOrDefault();
            if (item != null)
            {
                var ToEmail = db.Users.Where(x => x.Id == ToValue).Select(x => x.Email).FirstOrDefault();
                var IsEmailActive = db.Users.Where(x => x.Id == ToValue).Select(x => x.IsEmailNotificationActive).FirstOrDefault();
                //var SupplierEmail = db.Users.Where(x => x.Id == item.SupplierID).Select(x => x.Email).FirstOrDefault();
                var AttributeName = db.Users.Where(x => x.Id == item.ReqUID).Select(x => x.FirstName).FirstOrDefault();
                var userType = db.Users.Where(x => x.UserName == uname).Select(x => x.UserType).FirstOrDefault();
                CM_User_Notification model = new CM_User_Notification();
                model.orderStatus = item.Status;
                model.UserID = Convert.ToInt32(User.Identity.GetUserId());
                model.TypeName = userType;
                model.orderID = (int)orderid;
                model.PendingOrderStatus = false;
                model.AttributeName = AttributeName;
                model.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                model.IsViewed = false;
                model.Content = "Order" + " " + item.ORAPoNumber + "Has Been" + " " + item.Status;
                model.PendingDate = DateTime.Now.AddDays(30).ToString("MM/dd/yyyy");
                model.EntityID = Convert.ToInt32(ToValue);
                model.FromValue = User.Identity.Name;
                model.ToValue = ToEmail;
                db.CM_User_Notification.Add(model);
                db.SaveChanges();
                if (SendEmailNotification)
                mail.sendMail(ToEmail, "Order Creation Mail", model.Content);
            }
            return true;
        }
        private void AlertCallOff()
        {
            var callOffOrderDate = db.tblStock.Where(x => x.IsCallOff == true && x.CallOffStatus == false).Select(x => x.CallOffDate).ToList();

            var data = (from x in db.tblStock where x.IsCallOff == true && x.CallOffStatus == false select new { x.CallOffDate, x.PMOrderId }).ToList();

            foreach (var items in data)
            {
                if (DateTime.Now.Date <= Convert.ToDateTime(items.CallOffDate))
                {
                    DateTime date = Convert.ToDateTime(items.CallOffDate).AddDays(-7);
                    if (date == DateTime.Now.Date)
                    {
                        var userids = (from x in UnitOfWork.OrderRepository.GetAll() where x.ID == items.PMOrderId select new { x.SupplierID, x.Consolidator, x.ReqUID, x.OrgUID }).ToList();
                        foreach (var user in userids)
                        {
                            string usertype = UnitOfWork.UserRepository.Get(x => x.Id == user.ReqUID).UserType;
                            string FromValue = UnitOfWork.UserRepository.Get(x => x.Id == user.ReqUID).Email;
                            if (usertype == "Requisitioner")
                            {
                                var orderno = UnitOfWork.OrderRepository.Get(x => x.ID == items.PMOrderId).ORAPoNumber;
                                if (user.SupplierID > 0)
                                {
                                    SendNotification(items.PMOrderId, user.SupplierID);
                                }
                                if (user.Consolidator > 0)
                                {
                                    SendNotification(items.PMOrderId, user.SupplierID);
                                }
                                if (user.OrgUID > 0)
                                {
                                    SendNotification(items.PMOrderId, user.SupplierID);
                                }
                                if (user.ReqUID > 0)
                                {
                                    SendNotification(items.PMOrderId, user.SupplierID);
                                }
                                int adminId = UnitOfWork.UserRepository.Get(x => x.UserType == "Admin").Id;
                                if (adminId > 0)
                                {
                                    SendNotification(items.PMOrderId, adminId);
                                }
                            }
                        }
                    }
                    //OrderId.Add(Convert.ToInt32(items.PMOrderId));

                }

            }
        }
        public ActionResult Dashboard()
        {
            try
            {
                prndingOrder();
                AlertCallOff();
            }
            catch(Exception ex) {
                Console.Write(ex);
            }
            return View();
        }

        public ActionResult GetRainfallChart()
        {
            try
            {
                var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).ToString("MM/dd/yyyy");
                var monday = DateTime.Today.AddDays(+(int)DateTime.Today.DayOfWeek).ToString("MM/dd/yyyy");
                var listExpense = db.PM_Order.Where(x => x.CreatedDate.CompareTo(sunday) >= 0 && x.CreatedDate.CompareTo(monday) <= 0).ToList();
                var datedata = db.PM_Order.Select(x => x.CreatedDate).Distinct().ToList();
                List<int> count = new List<int>();
                foreach (var i in datedata)
                {
                    int count1 = db.PM_Order.Where(x => x.CreatedDate == i).Select(x => x.CreatedDate).Count();
                    count.Add(count1);
                }
                var chart = new Chart(width: 500, height: 300).
                    AddSeries(chartType: "bar",
                               xValue: datedata,
                               yValues: count)
                               .GetBytes("png");
                return File(chart, "image/bytes");
            }
            catch { return View(); }

        }

        public ActionResult CreatePie()
        {
            try
            {
                var datedata = db.PM_Order.Select(x => x.CreatedDate).Distinct().ToList();
                List<int> count = new List<int>();
                foreach (var i in datedata)
                {
                    int count1 = db.PM_Order.Where(x => x.CreatedDate == i).Select(x => x.CreatedDate).Count();
                    count.Add(count1);
                }
                var chart = new Chart(width: 300, height: 300)
                .AddSeries(chartType: "pie",
                                xValue: datedata,
                                yValues: count)
                                .GetBytes("png");
                return File(chart, "image/bytes");
            }
            catch { return View(); }

        }
        public ActionResult CreateLine()
        {
            try
            {
                var datedata = db.PM_Order.Select(x => x.CreatedDate).Distinct().ToList();
                List<int> count = new List<int>();
                foreach (var i in datedata)
                {
                    int count1 = db.PM_Order.Where(x => x.CreatedDate == i).Select(x => x.CreatedDate).Count();
                    count.Add(count1);
                }
                var chart = new Chart(width: 700, height: 300)
                .AddSeries(chartType: "line",
                                xValue: datedata,
                                yValues: count)
                                .GetBytes("png");
                return File(chart, "image/bytes");
            }
            catch { return View(); }
        }

        [HttpPost]
        public void SetEmailNotification(bool emailStatus)
        {
            var user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {

                user.IsEmailNotificationActive = emailStatus;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

            }

        }
    }
}
