using BlazorServerApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    [AttributeUsage(AttributeTargets.Property |
AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ValidIngredientsInRecipe : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (ValidIngredientsInRecipe.CustomValidation(value, validationContext))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Ingredients are invalid");
            }
        }

        public static bool CustomValidation(object value, ValidationContext validationContext)
        {
            var model = (DisplayRecipeModel)validationContext.ObjectInstance;

            foreach (DisplayIngredientInRecipeModel ingredient in model.Ingredients)
            {
                if (!ValidRecipeID.Validate(ingredient.IngredientID) || ingredient.Quantity is default(double) || !ValidUnit.Validate(ingredient.Unit))
                {
                    return false;
                }

            }
            return true;
        }
    }



    public sealed class ValidMethod : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ValidMethod.CustomValidation(value, validationContext))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Method is required");
            }
        }

        public static bool CustomValidation(object value, ValidationContext validationContext)
        {
            var model = (DisplayRecipeModel)validationContext.ObjectInstance;

            foreach (DisplayMethodModel ingredient in model.Method)
            {
                if (!ValidMethodText.ValidateMethodText(ingredient))
                {
                    return false;
                }

            }
            return true;
        }
    }

    public sealed class ValidMethodText : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (ValidMethodText.ValidateMethodText((DisplayMethodModel)validationContext.ObjectInstance))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Method is required");
            }
        }


        public static bool ValidateMethodText(DisplayMethodModel model)
        {
            //var model = (string ) validationContext.ObjectInstance;
            return true;
        }

    }


    [AttributeUsage(AttributeTargets.Property |
AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ValidMealType : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ValidMealType.ValidIngredient(value);

        }

        public static bool ValidIngredient(object value)
        {
            return DisplayRecipeModel.mealType.Contains(value);
        }

    }

    public class ValidRecipeID : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ValidRecipeID.Validate(value);
        }

        public static bool Validate(object value)
        {
            return value != null && uint.TryParse(value.ToString(), out _);
        }
    }

    public class ValidUnit : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ValidUnit.Validate(value);
        }

        public static bool Validate(object value)
        {
            return DisplayRecipeModel.SUPPORTEDUNITS.Contains(value);
        }
    }
}
