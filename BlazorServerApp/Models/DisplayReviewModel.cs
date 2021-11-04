using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class DisplayReviewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [MinLength(1, ErrorMessage = "Name is too short")]
        public string ReviewTitle { get; set; }
        public string ReviewText { get; set; }

        [Required]
        [Range(0,5)]
        public uint StarCount { get; set; }
    }

}
