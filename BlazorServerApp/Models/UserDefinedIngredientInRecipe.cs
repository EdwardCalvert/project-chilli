using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerApp.Models
{
    public class UserDefinedIngredientInRecipe
    {
        public uint? IngredientID { get; set; }
        public uint RecipeID { get; set; }
        public double Quantity { get; set; }
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
