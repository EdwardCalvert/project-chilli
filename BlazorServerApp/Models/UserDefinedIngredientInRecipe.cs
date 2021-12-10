using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Models
{
    public class UserDefinedIngredientInRecipe
    {
        [Required]
        public uint? IngredientID { get; set; }

        public uint RecipeID { get; set; }

        [Required, Range(0.01, 1000)]
        public double Quantity { get; set; }

        [Required, ValidUnit]
        public string Unit { get; set; }

        public string SqlInsertStatement()
        {
            return "INSERT INTO UserDefinedIngredientsInRecipe VALUES( @ingredientID, @recipeID, @quantity, @unit);";
        }

        public dynamic SqlAnonymousType(uint IngredientId, uint RecipeID)
        {
            return new { ingredientID = IngredientID, recipeID = RecipeID, quantity = Quantity, unit = Unit };
        }
    }
}