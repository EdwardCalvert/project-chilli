using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class DisplayPersonModel
    {
        [Required]
        [StringLength(15,ErrorMessage ="FirstName is too long.")]
        [MinLength(1,ErrorMessage ="FirstName is too short")]
            public string FirstName { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "FirstName is too long.")]
        [MinLength(1, ErrorMessage = "FirstName is too short")]
        public string LastName { get; set; }



        }
    }
