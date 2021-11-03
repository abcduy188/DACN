using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DCCovid.ViewModel
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Enter UserName")]
        [EmailAddress(ErrorMessage ="KHông đúng định dạng Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string PassWord { get; set; }
    }
}