using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogisticsSystem.Models
{
    public class ForgetPwdModel
    {
        [Required]
        [RegularExpression(@"^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
    }
}