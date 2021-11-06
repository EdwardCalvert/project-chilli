using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    [AttributeUsage(AttributeTargets.Property |
AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ValidIngredient : ValidationAttribute
    {
        public bool IsValid(DisplayRecipeModel value)
        {
            int count = 0;
            foreach (DisplayIngredientModel model in value.Ingredients)
            {
                if ((string.IsNullOrEmpty(model.Name) || model.Quantity is default(int) || model.Name == "Eric"))
                {
                    return false;
                }
                count++;
            }
            return true;

        }
    }
}
