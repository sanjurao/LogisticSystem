using Logistic.Base.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogisticSystem.Models
{
    public class EditPOModel
    {
        public EditPOModel()
        {
            Order = new PM_Order();
            stock = new tblStock();
            Items = new tblItem();
        }
        public PM_Order Order { get; set; }
        public tblStock stock { get; set; }
        public tblItem Items { get; set; }
    }
}