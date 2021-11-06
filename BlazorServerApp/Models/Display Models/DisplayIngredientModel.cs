using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class DisplayIngredientModel
    {
        [Required]
        [Range(0.01,1000) ]
        public double Quantity { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [ValidUnit]
        public string Unit { get; set; }

        public DisplayIngredientModel(int quantity, string name, string unit)
        {
            Quantity = quantity;
            Name = name;
            Unit = unit;
        }

        public DisplayIngredientModel() { }

       
    }

    public class ValidUnit : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return DisplayRecipeModel.SUPPORTEDUNITS.Contains(value);
        }
    }

    
}
