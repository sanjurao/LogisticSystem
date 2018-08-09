using Logistic.Base.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogisticsSystem.Models
{
    public class UserViewModel
    {
        [Required]       
        public string FirstName { get; set; }
            
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]       
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ReTypePwd { get; set; }

        public string PasswordSalt { get; set; }
        [Required]
        [RegularExpression(@"^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string TimeZone { get; set; }
        public Nullable<int> StartPage { get; set; }
        public string OldPassword { get; set; }
    
        public string FeedSelection { get; set; }
  
        public Gender Gender { get; set; }
     
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        public Nullable<int> ZipCode { get; set; }
        public bool isActive { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string UserType { get; set; }
        //[Required]
        //public bool Iagree { get; set; }
        public bool IsEmailNotificationActive { get; set; }
    }
}