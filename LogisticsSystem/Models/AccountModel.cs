using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogisticsSystem.Models
{
    public class AccountModel
    {
        public LoginViewModel login { get; set; }
        public UserViewModel UserReg { get; set; }
        public ForgetPwdModel ForgetPwd { get; set; }
    }
}