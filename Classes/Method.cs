﻿using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class Method
    {
        [Required]
        [MaxLength(63355, ErrorMessage = "Method too long for database")]
        public string MethodText { get; set; }

        public uint StepNumber { get; set; }

        public uint RecipeID { get; set; }

        public Method()
        {
        }

        public string SqlInsertStatement()
        {
            return $"INSERT INTO Method (StepNumber,RecipeID,MethodText) VALUES(@stepNumber, @recipeID,@methodText)";
        }

        public dynamic SqlAnonymousType()
        {
            return new { stepNumber = StepNumber, recipeID = RecipeID, methodText = MethodText };
        }

        public string SqlDeleteStatement()
        {
            return "DELETE FROM `RecipeDatabase`.`Method` WHERE  `StepNumber`=@stepNumber AND `RecipeID`=@recipeID;";
        }

        public dynamic SqlDeleteAnonymousType()
        {
            return new { stepNumber = StepNumber, recipeID = RecipeID };
        }
    }
}