using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

        public uint RecipeID { get; set; }
        public string FoodCode { get; set; }
        public string IngredientName { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public double Kcal { get; set; }
        public double Starch { get; set; }
        public double TotalSugars { get; set; }
        public double Glucose { get; set; }
        public double Fructose { get; set; }
        public double Sucrose { get; set; }
        public double Maltose { get; set; }
        public double Lactose { get; set; }
        public double Alchohol { get; set; }
        public double NSP { get; set; }
        public double SaturatedFattyAcids { get; set; }
        public double PolyunsaturatedFattyAcids { get; set; }
        public double MonounsaturatedFattyAcids { get; set; }
        public double Cholesterol { get; set; }

    public class ValidUnit : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return DisplayRecipeModel.SUPPORTEDUNITS.Contains(value);
        }
    }

    
}
