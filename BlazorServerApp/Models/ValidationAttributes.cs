using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
            var model = (Recipe)validationContext.ObjectInstance;

            foreach (UserDefinedIngredientInRecipe ingredient in model.Ingredients)
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
            var model = (Recipe)validationContext.ObjectInstance;

            foreach (Method ingredient in model.Method)
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
            if (ValidMethodText.ValidateMethodText((Method)validationContext.ObjectInstance))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Method is required");
            }
        }

        public static bool ValidateMethodText(Method model)
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
            return Recipe.mealType.Contains(value);
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
            return Recipe.SUPPORTEDUNITS.Contains(value);
        }
    }
}