using Logistic.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogisticSystem.Models
{
    public class NewPOOrder
    {
        public List<NewOrderItemDetail> NewOrderItemDetails { get; set; }
        public PM_Order NewOrderDetail { get; set; }

        public NewPOOrder()
        {
            NewOrderItemDetails = new List<NewOrderItemDetail>();
            //NewOrderItemDetails.Add(new NewOrderItemDetail() { CallOffDate = DateTime.Now.Date });
            NewOrderDetail = new PM_Order();
        }


    }
    public class NewOrderItemDetail
    {
        public DateTime CallOffDate { get; set; }
        public long txtTotalQuantity { get; set; }
        public int hfid { get; set; }
        public int? Id { get; set; }
        public string Category { get; set; }
        public string txtproduct { get; set; }
        public long txtQuntOrdered { get; set; }
        public long txtQuntShipped { get; set; }
        public long txtQuntLeft { get; set; }
        public decimal txtUnitValue { get; set; }
        public decimal txtAdditionalCost { get; set; }
        public decimal txtTotalValue { get; set; }
    }

}