using Lgistic.Base.UnitOfWorks;
using Logistic.Base.DataContext;
using Logistic.Base.Entity;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogisticSystem.Scheduling
{
    public class NotificationJob : IJob
    {
        UnitOfWork UnitOfWork = new UnitOfWork();
        LogisticsConnection db = new LogisticsConnection();
        public void AlertCallOff()
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
                                if (user.SupplierID > 0)
                                {
                                    CM_User_Notification model = new CM_User_Notification();
                                    model.UserID = Convert.ToInt32(user.ReqUID);
                                    model.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                                    model.IsViewed = false;
                                    model.PendingOrderStatus = false;
                                    model.PendingDate = DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy");
                                    var orderno = UnitOfWork.OrderRepository.Get(x => x.ID == items.PMOrderId).ORAPoNumber;
                                    model.Content = "Call Off Order" + orderno.ToString();
                                    model.EntityID = user.SupplierID;
                                    model.FromValue = FromValue;
                                    model.ToValue = UnitOfWork.UserRepository.Get(x => x.Id == user.SupplierID).Email;
                                    model.orderID = Convert.ToInt32(items.PMOrderId);
                                    db.CM_User_Notification.Add(model);
                                    db.SaveChanges();
                                }
                                if (user.Consolidator > 0)
                                {

                                    CM_User_Notification model = new CM_User_Notification();
                                    model.UserID = Convert.ToInt32(user.ReqUID);
                                    model.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                                    model.IsViewed = false;
                                    model.PendingOrderStatus = false;
                                    model.PendingDate = DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy");
                                    var orderno = UnitOfWork.OrderRepository.Get(x => x.ID == items.PMOrderId).ORAPoNumber;
                                    model.Content = "Call Off Order" + orderno.ToString();
                                    model.EntityID = user.SupplierID;
                                    model.FromValue = FromValue;
                                    model.ToValue = UnitOfWork.UserRepository.Get(x => x.Id == user.Consolidator).Email;
                                    model.orderID = Convert.ToInt32(items.PMOrderId);
                                    db.CM_User_Notification.Add(model);
                                    db.SaveChanges();
                                }
                                if (user.OrgUID > 0)
                                {
                                    CM_User_Notification model = new CM_User_Notification();
                                    model.UserID = Convert.ToInt32(user.ReqUID);
                                    model.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                                    model.IsViewed = false;
                                    model.PendingOrderStatus = false;
                                    model.PendingDate = DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy");
                                    var orderno = UnitOfWork.OrderRepository.Get(x => x.ID == items.PMOrderId).ORAPoNumber;
                                    model.Content = "Call Off Order" + orderno.ToString();
                                    model.EntityID = user.SupplierID;
                                    model.FromValue = FromValue;
                                    model.ToValue = UnitOfWork.UserRepository.Get(x => x.Id == user.OrgUID).Email;
                                    model.orderID = Convert.ToInt32(items.PMOrderId);
                                    db.CM_User_Notification.Add(model);
                                    db.SaveChanges();
                                }
                                if (user.ReqUID > 0)
                                {
                                    CM_User_Notification model = new CM_User_Notification();
                                    model.UserID = Convert.ToInt32(user.ReqUID);
                                    model.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                                    model.IsViewed = false;
                                    model.PendingOrderStatus = false;
                                    model.PendingDate = DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy");
                                    var orderno = UnitOfWork.OrderRepository.Get(x => x.ID == items.PMOrderId).ORAPoNumber;
                                    model.Content = "Call Off Order" + orderno.ToString();
                                    model.EntityID = user.SupplierID;
                                    model.FromValue = FromValue;
                                    model.ToValue = UnitOfWork.UserRepository.Get(x => x.Id == user.ReqUID).Email;
                                    model.orderID = Convert.ToInt32(items.PMOrderId);
                                    db.CM_User_Notification.Add(model);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    //OrderId.Add(Convert.ToInt32(items.PMOrderId));

                }

            }
        }
        public void Execute(IJobExecutionContext context)
        {
            AlertCallOff();
        }
    }
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<NotificationJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(4,42))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}