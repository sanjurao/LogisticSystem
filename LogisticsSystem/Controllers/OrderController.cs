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
using System.Text;
using LogisticSystem.SendMail;
using System.Threading.Tasks;
using System.ComponentModel;
using LogisticSystem.Models;
namespace LogisticsSystem.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        LogisticsConnection db = new LogisticsConnection();
        MailSending mail = new MailSending();

        [HttpPost]

        private bool SendNotification(long orderid, int ToValue)
        {
            string uname = (string)User.Identity.Name;
            var item = db.PM_Order.Where(x => x.ID == orderid).FirstOrDefault();
            if (item != null)
            {
                var ToEmail = db.Users.Where(x => x.Id == ToValue).Select(x => x.Email).FirstOrDefault();
                //var SupplierEmail = db.Users.Where(x => x.Id == item.SupplierID).Select(x => x.Email).FirstOrDefault();
                var AttributeName = db.Users.Where(x => x.Id == item.ReqUID).Select(x => x.FirstName).FirstOrDefault();
                var userType = db.Users.Where(x => x.UserName == uname).Select(x => x.UserType).FirstOrDefault();
                bool SendEmailNotification = Convert.ToBoolean(db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.IsEmailNotificationActive).FirstOrDefault());

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
                {
                    //var body = "<table style=color:green><tr><td colspan=3 color=Green align=center>Logistic System</td></tr><tr><td>User name</td><td>Status</td><td>PO No.</td><tr><td>" + User.Identity.Name + "</td><td>" + item.Status + "</td><td>" + item.MSD_PO + "</td></tr></table>";
                    var body = "<table style=color:green><tr><td colspan=3>Logistic System</td><td></td></tr><tr><td><ul><li>User Name</li><li>Status</li><li>PO Number</li></ul></td><td><ul><li>" + User.Identity.Name + "</li><li>" + item.Status + "</li><li>" + item.MSD_PO + "</li></ul></td></tr></table>";

                    mail.sendMail(ToEmail, "PO Status Notification", body);
                }

                PM_Order ordermodel = db.PM_Order.Where(x => x.ID == orderid).FirstOrDefault();
                ordermodel.IsActive = false;
                db.Entry(ordermodel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return true;
        }
        private void POHIstory(long PM_Order_Id, string Status, int supplierId)
        {
            tblPOrderHistory history = new tblPOrderHistory();
            history.Modificationdate = DateTime.Now.ToString("dd/MM/yyyy h:mm tt"); ;
            history.ModifiedBy = User.Identity.Name;
            history.PM_Order_Id = PM_Order_Id;
            history.Status = Status;
            history.SupplierId = supplierId;
            db.tblPOrderHistory.Add(history);
            db.SaveChanges();
        }
        private void Timeline(string MSD_PO, string Description)
        {
            tblTimeline timeline = new tblTimeline();
            timeline.PONo = MSD_PO;
            timeline.OrderDescription = Description;
            timeline.UserId = Convert.ToInt32(User.Identity.GetUserId());
            timeline.CreatedDate = DateTime.UtcNow.ToString("dd/MM/yyyy h:mm tt");
            db.tblTimeline.Add(timeline);
            db.SaveChanges();
        }
        public ActionResult Dashboard()
        {


            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SearchPO()
        {

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult PODetails(int? id)
        {
            if (id > 0)
            {
                ViewBag.id = id;

            }
            return View();
        }
        public ActionResult ViewAllMsg(string Uid)
        {

            if (Uid != null)
            {
                int toid = db.Users.Where(x => x.Email == Uid).Select(x => x.Id).FirstOrDefault();
                var item = db.tblMessage.Where(x => x.ToId == toid).ToList();
                if (item.Count > 0)
                {
                    foreach (var i in item)
                    {
                        var cm = db.tblMessage.Where(x => x.ID == i.ID).FirstOrDefault();
                        cm.Status = false;
                        db.Entry(cm).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("message");
            }
            return View();
        }
        public ActionResult Reply(int? Id)
        {
            if (Id > 0)
            {
                var data = db.tblMessage.Where(x => x.ID == Id).FirstOrDefault();
                return View(data);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Reply(int? Id, tblMessage model)
        {
            if (Id > 0)
            {
                var toid = db.tblMessage.Where(x => x.ID == Id).Select(x => x.FromId).FirstOrDefault();
                int fid = db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
                model.FromId = fid;
                model.ToId = toid;
                model.FromName = model.FromName;
                model.Date = System.DateTime.Now.ToString();
                model.Status = true;
                db.tblMessage.Add(model);
                db.SaveChanges();
                Response.Write("<script>alert('Sent Successfully.')</script>");
                return RedirectToAction("Message");
            }
            return View();
        }
        public ActionResult MessageDisplay()
        {
            return View();
        }

        public ActionResult InputMessage(int? Id)
        {
            return View();
        }

        public ActionResult Message(int? UId, string E)
        {
            if (UId > 0 && E == "V")
            {
                ViewBag.UId = UId;
            }
            else if (UId > 0 && E == "D")
            {
                tblMessage model = db.tblMessage.Find(UId);
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Message");
            }
            return View();
        }
        [HttpPost]

        public ActionResult Message(tblMessage model)
        {
            if (ModelState.IsValid)
            {
                int fid = db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
                var fromname = db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.FirstName).FirstOrDefault();
                model.FromId = fid;
                model.ToId = model.ToId;
                model.FromName = fromname;
                model.Date = System.DateTime.Now.ToString();
                model.Status = true;
                db.tblMessage.Add(model);
                db.SaveChanges();
                ModelState.Clear();
                Response.Write("<script>alert('Message Sent Successfully!')</script>");
            }
            return View();
        }
        public ActionResult ViewAll(string Uid)
        {
            if (Uid != null)
            {


                var item = db.CM_User_Notification.Where(x => x.ToValue == Uid).ToList();
                if (item.Count > 0)
                {
                    foreach (var i in item)
                    {
                        var cm = db.CM_User_Notification.Where(x => x.ID == i.ID).FirstOrDefault();
                        if (i.IsViewed == false)
                        {
                            tblTimeline model = new tblTimeline();
                            model.UserId = Convert.ToInt32(User.Identity.GetUserId());
                            model.NotificationContent = i.Content;
                            model.NotificationCreatedDate = i.CreatedOn;
                            model.NotificationFromValue = i.FromValue;
                            model.PONo = i.orderID.ToString();
                            db.tblTimeline.Add(model);
                            db.SaveChanges();
                        }
                        cm.IsViewed = true;
                        cm.IsSentInMail = true;
                        db.Entry(cm).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }


                return RedirectToAction("UserNotification");
            }
            return View();
        }
        public ActionResult PurchaseOrderView()
        {
            return View();
        }
        [HttpGet]
        public ActionResult RequitionerOrder(int? Id, int? CId)
        {
            var requser = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Requisitioner") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.requser = requser;
            var EndUser = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "EndUser") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.EndUser = EndUser;
            var Originator = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Originator") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.Originator = Originator;
            var Consolidater = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "consolidator") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.consol = Consolidater;
            var Supplier = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Supplier") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.supplier = Supplier;
            var Enduser = (from x in UnitOfWork.UserRepository.GetAll() select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.EndUser = Enduser;
            ViewBag.PID = Id;
            PM_Order data = UnitOfWork.OrderRepository.GetAll().Where(x => x.ID == Id).FirstOrDefault();
            int id = Convert.ToInt32(data.ApprovalTime);
            var apptime = db.tblApprovalTime.Where(x => x.Id == id).Select(x => x.ApprovalTime).FirstOrDefault();
            data.ApprovalTime = apptime.ToString();
            return View(data);
        }
        [HttpPost]
        public ActionResult RequitionerOrder(int? Id, PM_Order model, int[] SId, int[] ItemId, int[] txtQuntOrdered, int[] txtQuntShipped, int[] txtQuntLeft, decimal[] txtUnitValue, decimal[] txtAdditionalCost, decimal[] txtTotalValue)
        {
            if (User.Identity.Name != null)
            {
                for (int i = 0; i < SId.Length; i++)
                {
                    int StockId = Convert.ToInt32(SId[i]);
                    tblStock objMark = db.tblStock.Find(StockId);
                    objMark.ItemId = Convert.ToInt32(ItemId[i]);
                    objMark.QuntOrdered = Convert.ToInt32(txtQuntOrdered[i]);
                    objMark.QuntShipped = Convert.ToInt32(txtQuntShipped[i]);
                    objMark.QuntLeft = Convert.ToInt32(txtQuntLeft[i]);
                    objMark.UnitValue = Convert.ToDecimal(txtUnitValue[i]);
                    if (txtAdditionalCost != null)
                    {
                        objMark.AdditionalCost = Convert.ToDecimal(txtAdditionalCost[i]);
                    }
                    objMark.TotalValue = Convert.ToDecimal(txtTotalValue[i]);
                    if (txtQuntOrdered[i] == txtQuntShipped[i])
                    {
                        objMark.CallOffStatus = true;
                    }
                    db.SaveChanges();

                }
                string uname = (string)User.Identity.Name;
                var Userid = db.Users.Where(x => x.UserName == uname).Select(x => x.Email).FirstOrDefault();
                var Usertype = db.Users.Where(x => x.UserName == uname).Select(x => x.UserType).FirstOrDefault();
                var item = db.PM_Order.Where(x => x.ID == Id).FirstOrDefault();
                item.Status = model.Status;
                item.Location = Usertype;
                item.RecevedBy = model.RecevedBy;
                if (Id > 0 && model.Status == "Received" && model.RecevedBy != null)
                {
                    item.Close = true;
                }
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                POHIstory(model.ID, model.Status, model.SupplierID);
                int Uid = Convert.ToInt32(User.Identity.GetUserId());
                if (Uid != item.ReqUID)
                {
                    SendNotification(model.ID, Uid);
                }
                if (item.Consolidator > 0)
                {
                    int id = Convert.ToInt32(item.Consolidator);
                    SendNotification(model.ID, id);
                }

                if (item.EndUserId > 0)
                {
                    int id = Convert.ToInt32(item.EndUserId);
                    SendNotification(model.ID, id);
                }
                if (item.ReqUID > 0)
                {
                    int id = Convert.ToInt32(item.ReqUID);
                    SendNotification(model.ID, id);
                    Timeline(model.MSD_PO, model.Status);
                }
                if (item.OrgUID > 0)
                {
                    int id = Convert.ToInt32(item.OrgUID);
                    SendNotification(model.ID, id);
                }
                if (item.SupplierID > 0)
                {
                    int id = Convert.ToInt32(item.SupplierID);
                    SendNotification(model.ID, id);
                }

                if (item.Chargecode != null)
                {
                    int AdminID = UnitOfWork.UserRepository.Get(x => x.UserType == "Admin").Id;
                    SendNotification(model.ID, AdminID);
                }

                return RedirectToAction("PurchaseOrderView");

            }
            return View();
        }

        [HttpGet]
        public ActionResult NewOrder(int? Id, string E)
        {
            NewPOOrder poOrder = new NewPOOrder();
            try
            {
                var category = (from x in UnitOfWork.Categories_Repository.GetAll() select new SelectListItem { Value = x.Name.ToString(), Text = x.Name }).ToList();
                ViewBag.Category = category;

                var EndUser = (from x in UnitOfWork.UserRepository.GetAll() select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
                ViewBag.EndUser = EndUser;

                var Originator = (from x in UnitOfWork.UserRepository.GetAll() select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
                ViewBag.Originator = Originator;

                var Consolidater = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Consolidator") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
                ViewBag.consol = Consolidater;

                if (Id > 0 && E == "D")
                {
                    var stock = db.tblStock.Where(x => x.PMOrderId == Id).ToList();
                    foreach (var i in stock)
                    {
                        var stockdelete = db.tblStock.Where(x => x.tblStockId == i.tblStockId).FirstOrDefault();
                        db.Entry(stockdelete).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                }
                if (Id > 0 && E == "D")
                {
                    PM_Order Model = db.PM_Order.Find(Id);
                    db.Entry(Model).State = EntityState.Deleted;
                    db.SaveChanges();
                    return RedirectToAction("PurchaseOrder");
                }
                else if (Id > 0)
                {
                    List<tblStock> liststock = db.tblStock.Where(x => x.PMOrderId == Id).ToList();
                    foreach (var item in liststock)
                    {
                        poOrder.NewOrderItemDetails.Add(new NewOrderItemDetail
                        {
                            Id = item.tblStockId,
                            txtproduct = db.tblItem.Where(x => x.Id == item.ItemId).First().ItemName,
                            txtQuntOrdered = item.QuntOrdered,
                            txtTotalQuantity = item.TotalQty,
                            txtQuntShipped = item.QuntShipped,
                            txtQuntLeft = item.QuntLeft,
                            txtUnitValue = item.UnitValue,
                            txtAdditionalCost = item.AdditionalCost,
                            CallOffDate = Convert.ToDateTime(item.CallOffDate),
                            txtTotalValue = item.TotalValue,
                        });
                    }
                    var itemid = db.tblStock.Where(x => x.PMOrderId == Id).Select(x => x.ItemId).FirstOrDefault();
                    var items = db.tblItem.Where(x => x.Id == itemid).Select(x => new { x.Id, x.ItemName }).ToList();
                    ViewBag.Name = items;
                    poOrder.NewOrderDetail = UnitOfWork.OrderRepository.GetAll().Where(x => x.ID == Id).FirstOrDefault();
                    //return View(poOrder);
                }

            }
            catch { }
            return View(poOrder);

        }


        [HttpPost]
        public ActionResult NewOrder(NewPOOrder newPOorder, HttpPostedFileBase file, int? Id)
        {
            try
            {
                PM_Order newOrder = newPOorder.NewOrderDetail;
                List<NewOrderItemDetail> newOrderItemDetails = newPOorder.NewOrderItemDetails;

                if (Id > 0)
                {

                    var RId = db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
                    var usertype = UnitOfWork.UserRepository.Get(x => x.Id == RId).UserType;
                    var editOrder = db.PM_Order.Where(x => x.ID == Id).FirstOrDefault();
                    editOrder.ORAPoNumber = newOrder.ORAPoNumber;
                    editOrder.ReqUID = RId;
                    if (file != null && file.ContentLength > 0)
                    {
                        string path = Path.Combine(Server.MapPath("~/files"),
                         Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        editOrder.pdffile = file.FileName;
                    }

                    editOrder.CategoryID = "";
                    editOrder.ReqUID = RId;
                    editOrder.EndUserId = newOrder.EndUserId;
                    editOrder.OrgUID = newOrder.OrgUID;
                    editOrder.Consolidator = newOrder.Consolidator;
                    var supplierid = db.tblSupplier.Where(x => x.Id == newOrder.SupplierID).Select(x => x.SupplierID).FirstOrDefault();
                    editOrder.SupplierID = Convert.ToInt32(supplierid);
                    editOrder.Description = newOrder.Description;
                    editOrder.MOT = newOrder.MOT;
                    editOrder.IsActive = true;
                    editOrder.Location = usertype;
                    editOrder.CheckStatus = false;
                    editOrder.Theme = newOrder.Theme;
                    editOrder.NeedByDate = newOrder.NeedByDate;
                    editOrder.DueDate = newOrder.DueDate;
                    editOrder.Calloff = newOrder.Calloff;
                    editOrder.MSD_PO = newOrder.MSD_PO + "-" + System.DateTime.UtcNow;
                    editOrder.LeadTime = newOrder.LeadTime;
                    editOrder.Chargecode = newOrder.Chargecode;
                    editOrder.Status = newOrder.Status;
                    editOrder.ApprovalTime = newOrder.ApprovalTime;
                    editOrder.KPI = newOrder.KPI;
                    editOrder.AirwayBillNumber = newOrder.AirwayBillNumber;
                    editOrder.RecevedBy = newOrder.RecevedBy;
                    editOrder.CreatedDate = System.DateTime.Now.ToString("MM/dd/yyyy");
                    editOrder.IsActive = true;
                    editOrder.CheckStatus = false;
                    db.Entry(editOrder).State = EntityState.Modified;
                    db.SaveChanges();
                    foreach (var item in newOrderItemDetails)
                    {
                        var stock = db.tblStock.Where(x => x.tblStockId == item.Id).FirstOrDefault();
                        stock.QuntOrdered = item.txtQuntOrdered;
                        stock.TotalQty = item.txtTotalQuantity;
                        UpdateProduct(item.txtproduct, stock.ItemId);
                        if (item.CallOffDate != null)
                        {
                            stock.CallOffDate = item.CallOffDate.ToShortDateString();
                        }
                        stock.TotalValue = item.txtTotalValue;
                        stock.UnitValue = item.txtUnitValue;
                        db.Entry(stock).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    ModelState.Clear();
                    return RedirectToAction("PurchaseOrder");


                }
                else
                {
                    var RId = db.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
                    var usertype = UnitOfWork.UserRepository.Get(x => x.Id == RId).UserType;
                    PM_Order order = new PM_Order();
                    order.ORAPoNumber = newOrder.ORAPoNumber;
                    if (file != null && file.ContentLength > 0)
                    {
                        string path = Path.Combine(Server.MapPath("~/files"),
                         Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        order.pdffile = file.FileName;
                    }

                    order.CategoryID = "";
                    order.ReqUID = RId;
                    order.EndUserId = newOrder.EndUserId;
                    order.OrgUID = newOrder.OrgUID;
                    order.Consolidator = newOrder.Consolidator;
                    var supplierid = db.tblSupplier.Where(x => x.Id == newOrder.SupplierID).Select(x => x.SupplierID).FirstOrDefault();
                    order.SupplierID = Convert.ToInt32(supplierid);
                    order.Description = newOrder.Description;
                    order.MOT = newOrder.MOT;
                    order.IsActive = true;
                    order.Location = usertype;
                    order.CheckStatus = false;
                    order.Theme = newOrder.Theme;
                    order.NeedByDate = newOrder.NeedByDate;
                    order.DueDate = newOrder.DueDate;
                    order.Calloff = newOrder.Calloff;
                    order.MSD_PO = newOrder.MSD_PO + "-" + System.DateTime.UtcNow;
                    order.LeadTime = newOrder.LeadTime;
                    order.Chargecode = newOrder.Chargecode;
                    order.Status = newOrder.Status;
                    order.ApprovalTime = newOrder.ApprovalTime;
                    order.KPI = newOrder.KPI;
                    order.AirwayBillNumber = newOrder.AirwayBillNumber;
                    order.RecevedBy = newOrder.RecevedBy;
                    order.CreatedDate = System.DateTime.Now.ToString("MM/dd/yyyy");
                    UnitOfWork.OrderRepository.Insert(order);
                    POHIstory(order.ID, order.Status, order.SupplierID);

                    foreach (var item in newOrderItemDetails)
                    {
                        tblStock stock = new tblStock();
                        stock.PMOrderId = order.ID;
                        stock.TotalQty = item.txtTotalQuantity;
                        if (item.CallOffDate != null)
                        {
                            stock.CallOffDate = item.CallOffDate.ToShortDateString();
                        }
                        stock.ItemId = SaveProduct(item.txtproduct, null);
                        stock.QuntOrdered = item.txtQuntOrdered;
                        stock.UnitValue = item.txtUnitValue;
                        stock.TotalValue = item.txtTotalValue;
                        stock.IsCallOff = newOrder.Calloff;
                        db.tblStock.Add(stock);
                        db.SaveChanges();
                    }
                    if (RId > 0)
                    {
                        Timeline(order.MSD_PO, order.Description);
                    }
                    Response.Write("<script>alert('Created Sussfully')</script>");
                    ModelState.Clear();
                    return RedirectToAction("PurchaseOrder", "Order");
                }

            }
            catch(Exception ex) {
                Console.Write(ex);
            }
            var category = (from x in UnitOfWork.Categories_Repository.GetAll() select new SelectListItem { Value = x.Name.ToString(), Text = x.Name }).ToList();
            ViewBag.Category = category;

            var EndUser = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "EndUser" || x.UserType == "Requisitioner") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.EndUser = EndUser;

            var Consolidater = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Consolidator") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.consol = Consolidater;
            return View();
        }

        public void UpdateProduct(string productName, int prodId)
        {
            var item = db.tblItem.Where(x => x.Id == prodId).FirstOrDefault();
            item.ItemName = productName;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();

        }
        public int SaveProduct(string productName, int? id)
        {
            var productId = 0;

            tblItem model = new tblItem();
            model.ItemName = productName.Trim();
            if (id > 0)
            {
                var cm = db.tblItem.Where(x => x.Id == id).FirstOrDefault();
                cm.Id = Convert.ToInt32(id);
                cm.ItemName = model.ItemName;
                cm.ItemDesc = model.ItemDesc;
                cm.UOM = model.UOM;
                db.Entry(cm).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                model.ItemName = model.ItemName;
                model.ItemDesc = model.ItemDesc;
                model.UOM = model.UOM;
                model.Status = true;
                db.tblItem.Add(model);
                db.SaveChanges();
                productId = model.Id;
            }


            return productId;
        }


        //[NonAction]
        //private static NewOrderItemDetail NewOrderItemModel()
        //{
        //    return new NewOrderItemDetail
        //    {

        //    };
        //}

        //[HttpGet]
        //public virtual PartialViewResult OrderItem()
        //{
        //    var model = NewOrderItemModel();
        //    return PartialView("_OrderItem", model);
        //}

        public ActionResult Download(int? id)
        {
            string Filename = UnitOfWork.OrderRepository.Get(x => x.ID == id).pdffile;
            string path = Path.Combine(@"~/files/", Filename);
            return File(path, "application/pdf");
        }
        // Order Report
        [HttpGet]
        public ActionResult OrderReport()
        {
            return View();
        }
        // Report View
        [HttpGet]
        public ActionResult ReportView(int? id)
        {
            if (id > 0)
            {
                ViewBag.id = id;
            }
            return View();
        }
        // Purchase Order
        [HttpGet]
        public ActionResult PurchaseReport()
        {
            return View();
        }
        // Purchase Report view
        [HttpGet]
        public ActionResult PurchaseReportView(int? id)
        {
            if (id > 0)
            {
                ViewBag.id = id;

            }
            return View();
        }

        [HttpGet]
        public ActionResult View(int? Id, int? CId, int? orderid)
        {
            var requser = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Requisitioner") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.requser = requser;
            var EndUser = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "EndUser") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.EndUser = EndUser;

            var Originator = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Originator") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.Originator = Originator;
            var Consolidater = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "consolidator") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.consol = Consolidater;

            var Supplier = (from x in UnitOfWork.UserRepository.GetAll().Where(x => x.UserType == "Supplier") select new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName }).ToList();
            ViewBag.supplier = Supplier;

            var approvaltime = (from x in db.tblApprovalTime select new SelectListItem { Value = x.Id.ToString(), Text = x.ApprovalTime }).ToList();
            ViewBag.approvaltime = approvaltime;
            var theme = (from x in db.tblTheme select new SelectListItem { Value = x.ID.ToString(), Text = x.Name }).ToList();
            ViewBag.theme = theme;

            var pendingorder = db.CM_User_Notification.Where(x => x.ID == Id).ToList();

            ViewBag.PID = Id;
            PM_Order data = UnitOfWork.OrderRepository.GetAll().Where(x => x.ID == Id).FirstOrDefault();
            int id = Convert.ToInt32(data.ApprovalTime);
            var apptime = db.tblApprovalTime.Where(x => x.Id == id).Select(x => x.ApprovalTime).FirstOrDefault();
            data.ApprovalTime = apptime;
            if (orderid > 0)
            {
                return View();
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult View(long Id, PM_Order model)
        {
            try
            {
                if (model.Status == "Shipped_To_Consilidater")
                {
                    var orderid = db.tblPOrderHistory.Where(x => x.Status == "Accept" && x.SupplierId == model.SupplierID).Select(x => x.PM_Order_Id).ToList();
                    if (orderid.Count() > 1)
                    {
                        double diff = 0.0;
                        foreach (var item in orderid)
                        {

                            int modificationdate = Convert.ToDateTime(db.tblPOrderHistory.Where(x => x.Status == "Accept" && x.PM_Order_Id == item).Select(x => x.Modificationdate).FirstOrDefault()).Day;
                            int modificationdate1 = Convert.ToDateTime(db.tblPOrderHistory.Where(x => x.Status == "Shipped_To_Consilidater" && x.PM_Order_Id == item).Select(x => x.Modificationdate).FirstOrDefault()).Day;
                            diff += (modificationdate1 - modificationdate);
                        }
                        double ladtime = diff / orderid.Count();
                        tblSupplier supplier = db.tblSupplier.Where(x => x.SupplierID == model.SupplierID).FirstOrDefault();
                        supplier.LeadTime = ladtime.ToString();
                        db.Entry(supplier).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                if (User.Identity.Name != null)
                {
                    string uname = (string)User.Identity.Name;
                    var Userid = db.Users.Where(x => x.UserName == uname).Select(x => x.Email).FirstOrDefault();
                    var usertype = db.Users.Where(x => x.UserName == uname).Select(x => x.UserType).FirstOrDefault();
                    if (Id > 0)
                    {
                        var item = db.PM_Order.Where(x => x.ID == Id).FirstOrDefault();
                        item.Location = usertype;
                        item.Status = model.Status;
                        item.Volume = model.Volume;
                        item.FreightCost = model.FreightCost;
                        item.DestinationCharges = model.DestinationCharges;
                        item.EcowasCharges = model.EcowasCharges;
                        item.Grandtotal = model.Grandtotal;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        POHIstory(model.ID, model.Status, model.SupplierID);
                        Timeline(item.MSD_PO, item.Description);
                        if (item.Consolidator > 0)
                        {
                            int toid = Convert.ToInt32(item.Consolidator);
                            SendNotification(Id, toid);
                        }
                        if (item.ReqUID > 0)
                        {
                            int toid = Convert.ToInt32(item.ReqUID);
                            SendNotification(Id, toid);

                        }
                        if (item.EndUserId > 0)
                        {
                            int toid = Convert.ToInt32(item.EndUserId);
                            SendNotification(Id, toid);

                        }
                        if (item.OrgUID > 0)
                        {
                            int toid = Convert.ToInt32(item.OrgUID);
                            SendNotification(Id, toid);

                        }
                        if (item.SupplierID > 0)
                        {
                            int toid = Convert.ToInt32(item.SupplierID);
                            SendNotification(Id, toid);
                        }

                        int AdminId = UnitOfWork.UserRepository.Get(x => x.UserType == "Admin").Id;
                        if (AdminId != null)
                        {
                            SendNotification(Id, AdminId);
                        }
                    }

                }

                return RedirectToAction("ViewPO");
            }
            catch { }
            return View();
        }

        public ActionResult Delete(int id)
        {
            PM_UserOrder order = UnitOfWork.UserOrderRepository.Get(x => x.ID == id);
            LogisticsConnection db = new LogisticsConnection();

            int noOfRowDeleted = db.Database.ExecuteSqlCommand("delete from PM_UserOrder where ID='" + id + "'");
            Response.Write("<script>alert(''" + noOfRowDeleted + "'Row deleted')</script>");
            return RedirectToAction("CreatePO", "Order", null);
        }

        public ActionResult PurchasingOrder()
        {
            return View();
        }
        [HttpGet]
        public ActionResult PurchaseView(int? id)
        {
            if (id > 0)
            {
                ViewBag.Id = id;
            }
            return View();

        }

        public ActionResult PurchaseOrder()
        {
            var Orderdata = UnitOfWork.OrderRepository.GetAll().ToList().OrderByDescending(x => x.CreatedDate);
            return View(Orderdata);
        }
        public ActionResult ViewPO()
        {
            return View();
        }
        [HttpGet]
        public ActionResult OrderSupply(PM_Supply model, int? Id)
        {
            string PO = UnitOfWork.OrderRepository.Get(x => x.ID == Id).ORAPoNumber;
            model.ORAPoNumber = PO;

            return View(model);
        }
        [HttpPost]
        public ActionResult OrderSupply(PM_Supply model)
        {
            model.POID = model.ID;
            UnitOfWork.SupplyRepository.Insert(model);
            ModelState.Clear();
            return View("PurchasingOrder");
        }
        //public ActionResult ProcurementAdviceNote()
        //{
        //    return View();
        //}
        //public ActionResult ProcurementEnquiry()
        //{
        //    return View();
        //}
        //public ActionResult Receipting()
        //{
        //    return View();
        //}
        //public ActionResult Stock()
        //{
        //    return View();
        //}
        [HttpGet]
        public ActionResult SendPO(int? Id, int? Cid)
        {
            var item = db.Users.Where(x => x.Id == Id).FirstOrDefault();
            if (item != null)
            {
                var Supplieremail = db.Users.Where(x => x.Email == item.Email).Select(x => x.Email).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("NewOrder", "Order");
            }
            return View("NewOrder");
        }
        [HttpPost]
        public ActionResult SendPO()
        {

            return View();
        }
        public ActionResult Items()
        {
            return View();
        }
        //public ActionResult Send(int Uid)
        //{
        //    if (User.Identity.Name != null)
        //    {
        //        string uname = (string)User.Identity.Name;
        //        if (Uid > 0)
        //        {
        //            int Userid = db.Users.Where(x => x.UserName == uname).Select(x => x.Id).FirstOrDefault();

        //            var item = db.PM_UserOrder.Where(x => x.ID == Uid).FirstOrDefault();
        //            if (item != null)
        //            {
        //                //var FromUserEMailId = db.Users.Where(x => x.UserName == uname).Select(x => x.Email).FirstOrDefault();
        //                var OriginaterEmil = db.Users.Where(x => x.Id == item.OriginaterID).Select(x => x.Email).FirstOrDefault();
        //                var AttributeName = db.Users.Where(x => x.Id == item.OriginaterID).Select(x => x.UserName).FirstOrDefault();
        //                var userType = db.Users.Where(x => x.UserName == uname).Select(x => x.UserType).FirstOrDefault();
        //                CM_User_Notification model = new CM_User_Notification();
        //                model.UserID = Userid;
        //                model.TypeName = userType;
        //                model.AttributeName = AttributeName;
        //                model.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
        //                model.IsViewed = false;
        //                model.Content = item.Comments;
        //                model.EntityID = item.OriginaterID;
        //                model.FromValue = User.Identity.Name;
        //                model.ToValue = OriginaterEmil;
        //                db.CM_User_Notification.Add(model);
        //                db.SaveChanges();
        //                PM_UserOrder ordermodel = db.PM_UserOrder.Where(x => x.ID == Uid).FirstOrDefault();
        //                ordermodel.Status = true;
        //                ordermodel.IsActive = false;
        //                db.Entry(ordermodel).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }

        //        }


        //    }

        //    return RedirectToAction("CreatePO", "Order", null);
        //}
        public ActionResult SendData(int Cid, int Sid, int Id, long orderid, int Eid, int orgId)
        {
            try
            {
                int id = Convert.ToInt32(User.Identity.GetUserId());
                if (User.Identity.Name != null)
                {
                    string uname = (string)User.Identity.Name;
                    if (Eid > 0)
                    {
                        SendNotification(orderid, Eid);
                    }
                    if (Cid > 0)
                    {
                        SendNotification(orderid, Cid);
                    }
                    if (orgId > 0)
                    {
                        SendNotification(orderid, orgId);
                    }
                    if (Sid > 0)
                    {
                        SendNotification(orderid, Sid);
                    }
                }

                if (id > 0)
                {
                    SendNotification(orderid, id);
                }
                var Email = UnitOfWork.UserRepository.Get(x => x.UserType == "Admin").Email;
                if (Email != null)
                {
                    int ID = UnitOfWork.UserRepository.Get(x => x.Email == Email).Id;
                    SendNotification(orderid, ID);

                }


            }
            catch { }
            return RedirectToAction("PurchaseOrder", "Order", null);
        }
        [HttpGet]
        public ActionResult UserNotification(int? Id, string E, int? RId, int? CId, int? UId, int? VId)
        {
            try
            {

                if (UId != null)
                {
                    var item = db.CM_User_Notification.Where(x => x.ToValue == UId.ToString()).ToList();
                    if (item.Count > 0)
                    {
                        foreach (var i in item)
                        {
                            var cm = db.CM_User_Notification.Where(x => x.ID == i.ID).FirstOrDefault();
                            cm.IsSentInMail = true;
                            db.Entry(cm).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("UserNotification");
                }

                if (Id > 0 && RId > 0 && E == "D" && CId > 0)
                {
                    var item = db.CM_User_Notification.Where(x => x.ID == Id && x.UserID == RId && x.ActorID == CId && x.IsViewed == false).FirstOrDefault();
                    if (item != null)
                    {
                        var endusermail = db.Users.Where(x => x.Id == item.ActorID).Select(x => x.Email).FirstOrDefault();
                        CM_User_Notification model = db.CM_User_Notification.Where(x => x.ID == Id).FirstOrDefault();
                        model.IsViewed = false;
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        CM_User_Notification cmmodel = new CM_User_Notification();
                        cmmodel.UserID = item.UserID;
                        cmmodel.EntityID = item.EntityID;
                        cmmodel.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                        cmmodel.IsViewed = true;
                        cmmodel.ToValue = endusermail;
                        cmmodel.FromValue = User.Identity.Name;
                        cmmodel.Content = "Order '" + item.ID + "' Approved";
                        db.CM_User_Notification.Add(cmmodel);
                        db.SaveChanges();
                    }
                }

                else if (Id > 0 && RId > 0 && E == "D")
                {
                    var item = db.CM_User_Notification.Where(x => x.ID == Id && x.UserID == RId && x.IsViewed == false).FirstOrDefault();
                    if (item != null)
                    {
                        var chkpmorder = db.PM_Order.Where(x => x.ReqUID == RId && x.CheckStatus == false).FirstOrDefault();
                        if (chkpmorder != null)
                        {
                            PM_Order Pmorder = db.PM_Order.Where(x => x.ReqUID == RId).FirstOrDefault();
                            Pmorder.CheckStatus = true;
                            db.Entry(Pmorder).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var endusermail = db.Users.Where(x => x.Id == item.UserID).Select(x => x.Email).FirstOrDefault();
                        CM_User_Notification model = db.CM_User_Notification.Where(x => x.ID == Id).FirstOrDefault();
                        model.IsViewed = true;
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        CM_User_Notification cmmodel = new CM_User_Notification();
                        cmmodel.UserID = item.UserID;
                        cmmodel.EntityID = item.EntityID;
                        cmmodel.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                        cmmodel.IsViewed = true;
                        cmmodel.ToValue = endusermail;
                        cmmodel.FromValue = User.Identity.Name;
                        cmmodel.Content = "Order '" + item.ID + "' Approved";
                        db.CM_User_Notification.Add(cmmodel);
                        db.SaveChanges();
                        PM_UserOrder UserOrder = db.PM_UserOrder.Where(x => x.OriginaterID == item.EntityID).FirstOrDefault();
                        UserOrder.IsActive = true;
                        db.Entry(UserOrder).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                else if (Id > 0 && RId > 0)
                {
                    var item = db.CM_User_Notification.Where(x => x.ID == Id && x.UserID == RId && x.IsViewed == false).FirstOrDefault();
                    if (item != null)
                    {
                        var endusermail = db.Users.Where(x => x.Id == item.UserID).Select(x => x.Email).FirstOrDefault();
                        CM_User_Notification model = db.CM_User_Notification.Where(x => x.ID == Id).FirstOrDefault();
                        model.IsViewed = true;
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        CM_User_Notification cmmodel = new CM_User_Notification();
                        cmmodel.UserID = item.UserID;
                        cmmodel.EntityID = item.EntityID;
                        cmmodel.CreatedOn = System.DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
                        cmmodel.IsViewed = true;
                        cmmodel.ToValue = endusermail;
                        cmmodel.FromValue = User.Identity.Name;
                        cmmodel.Content = "Order '" + item.ID + "' Approved";
                        db.CM_User_Notification.Add(cmmodel);
                        db.SaveChanges();
                    }
                }
                else if (UId > 0)
                {
                    ViewBag.UId = UId;
                }
                else if (VId > 0)
                {
                    ViewBag.VId = VId;
                }

            }
            catch { return View(); }
            return View();
        }

        public JsonResult SelectLeadTime(int? Id)
        {
            var LeadeTime = db.tblSupplier.Where(x => x.Id == Id).Select(x => x.LeadTime).FirstOrDefault();
            return Json(LeadeTime, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectDuration(int? id)
        {
            var duration = db.tblMOT.Where(x => x.ID == id).Select(x => x.Duration).FirstOrDefault();
            return Json(duration, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckORAPoNumberExisit(string ORAPoNumber)
        {
            var orapoNumber = db.PM_Order.Where(x => string.Compare(x.ORAPoNumber.Trim(), ORAPoNumber.Trim(), true) == 0).Count();
            return Json(orapoNumber, JsonRequestBehavior.AllowGet);
        }
    }
}
