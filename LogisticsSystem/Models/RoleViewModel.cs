using Logistic.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogisticSystem.Models
{
    public class RoleViewModel
    {
        public RoleViewModel()
        {
            usertype = new List<tblUserType>();
            menues = new List<tblmenues>();
            Role = new List<tblRole>();
        }
        public List<tblUserType> usertype { get; set; }
        public List<tblmenues> menues { get; set; }
        public List<tblRole> Role { get; set; }
    }
}