using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class CreateNewUser
    {
        //public CreateNewUser()
        ////{
        ////    Role = "Administrator";
        ////}
        [Required]
        public string UserName { get; set; }
        [Required, MinLength(4, ErrorMessage = "Please use a longer password!")]
        public string Password { get; set; }
        [Required]
        public string RepeatPassword { get; set; }
        [Required]
        public string Role { get; set; }
    }

}
