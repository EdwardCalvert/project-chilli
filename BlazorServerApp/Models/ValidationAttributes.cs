using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BlazorServerApp.Models
{
    public sealed class ListLengthGreaterThanZero : ValidationAttribute
    {
        protected override ValidationResult IsValid(object v, ValidationContext validationContext)
        {
            List<object> list = new List<object>();
            IList value = v as IList;

            if (value != null && value.GetType().IsGenericType)
            {
                list = value.Cast<object>().ToList();
                if (list.Count > 0)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult($"{validationContext.DisplayName} must have more items than 0");
        }
    }



    public sealed class ValidNutritionalElement : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ValidMethod.CustomValidation(value, validationContext))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Nutritional element is required");
            }
        }

        public static bool CustomValidation(object value, ValidationContext validationContext)
        {
            if (double.TryParse(value.ToString(), out _))
            {
                return true;
            }
            return false;
        }
    }

    public sealed class ValidDifficulty : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ValidMethod.CustomValidation(value, validationContext))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Difficulty is required");
            }
        }

        public static bool CustomValidation(object value, ValidationContext validationContext)
        {
            try
            {
                if (Recipe.DifficultyEnum.ContainsKey((string)validationContext.ObjectInstance))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
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
            if (model.MethodText != null && model.MethodText.Length < DatabaseConstants.VarCharMax)
            {
                return true;
            }
            return false;
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

    //public class ValidUnit : ValidationAttribute
    //{
    //    public override bool IsValid(object value)
    //    {
    //        return ValidUnit.Validate(value);
    //    }

    //    public static bool Validate(object value)
    //    {
    //        return Recipe.SUPPORTEDUNITS.Contains(value);
    //    }
    //}
}