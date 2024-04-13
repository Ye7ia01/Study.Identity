using Identity.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(3)]
        public string UserName { get; set; }

        [Remote(action: "IsEmailValid", controller: "Account", ErrorMessage = "Email is Already Taken")]
        [Required(ErrorMessage = "Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]

        public string UserPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("UserPassword",ErrorMessage ="Password & Confirmation Do not match")]
        public string UserConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone is Required")]
        [DataType(DataType.PhoneNumber)]
        
        public string UserPhone { get; set; }

        public AccountTypes AccountType { get; set; } = AccountTypes.Customer;
    }
}
