using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace LogisticsSystem.Models
{
    public class OrderViewModel
    {

        public Nullable<int> UserID { get; set; }
        [Required]
        public Nullable<System.DateTime> DateRequired { get; set; }
        [Required]
        public string RequisitionerId { get; set; }
        [Required]
        public string Comments { get; set; }
        [Required]
        public HttpPostedFileBase file { get; set; }
    }

}
