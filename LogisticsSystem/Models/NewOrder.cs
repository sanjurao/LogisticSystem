using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogisticsSystem.Models
{
    public class NewOrder
    {
        [Required]
        public string PoNumber { get; set; }
        [Required]
        public long ReqUID { get; set; }
        [Required]
        public long OrgUID { get; set; }
        [Required]
        public Nullable<long> Consolidator { get; set; }
        [Required]
        public int SupplierID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Nullable<System.DateTime> NeedByDate { get; set; }
        [Required]
        public Nullable<System.DateTime> DueDate { get; set; }
        [Required]
        public Nullable<int> Status { get; set; }
        [Required]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Count must be a natural number")]
        public Nullable<long> MOT { get; set; }
        public string Budget { get; set; }
        public Nullable<long> CategoryID { get; set; }
        public string Grandtotal { get; set; }
        public string Theme { get; set; }
        public Nullable<bool> Local { get; set; }
        public string KPI { get; set; }
        public string Calloff { get; set; }
        public string MSD_PO { get; set; }
        public Nullable<long> LeadTime { get; set; }
        public Nullable<int> Chargecode { get; set; }
    }
}